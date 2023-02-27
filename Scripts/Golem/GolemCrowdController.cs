using System;
using System.Collections;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using GrandmaGreen.Garden;
using Core.Input;
using UnityEngine.Tilemaps;

namespace GrandmaGreen.Entities
{
    public class GolemCrowdController : MonoBehaviour
    {
        [Header("Golem Management")]
        public GolemManager golemManager;
        public int happinessChangeValue = -10;

        [Header("Golem References")]
        public GardenAreaController gardenArea;
        public PointerState pointerState;
        [SerializeField] TileStore tileStore;

        [Header("Settings")]
        [SerializeField] float validCheckTime = 0.05f;
        public Color invalidColor;

        bool returnFromPause = true;
        public void Awake()
        {
            EventManager.instance.EVENT_ASSIGN_TASK += AssignGolemAction;
            EventManager.instance.EVENT_GOLEM_DRAG += EnterDragGolem;
            golemManager.golemWorkTimer.Pause();
        }

        public void OnDestroy()
        {
            EventManager.instance.EVENT_ASSIGN_TASK -= AssignGolemAction;
            EventManager.instance.EVENT_GOLEM_DRAG -= EnterDragGolem;
        }

        void OnEnable() {
            golemManager.LoadGolemData();
            golemManager.golemWorkTimer.Resume(true);
            golemManager.golemWorkTimer.onTick += GolemDoAction;
            golemManager.tiles = gardenArea.tilemap;
        }

        void OnDisable() {
            golemManager.SaveGolemData();
            golemManager.golemWorkTimer.Pause();
            golemManager.golemWorkTimer.onTick -= GolemDoAction;
        }

        [Header("Debug Options")]
        public CharacterId theGolem = CharacterId.Pumpkin;
        public int value = 0;

        [Button(ButtonSizes.Medium)]
        public void UpdateGolemHappiness()
        {
            EventManager.instance.HandleEVENT_GOLEM_HAPPINESS_UPDATE((ushort)theGolem, value);
        }

        [Button(ButtonSizes.Medium)]
        public void SpawnTulipGolem()
        {
            ushort id = (ushort)CharacterId.Tulip;
            Vector3 pos = new Vector3(0,0,0);
            golemManager.OnGolemSpawn(id, pos);
        }

        [Button(ButtonSizes.Medium)]
        public void WaterGolem()
        {
            GolemDoAction(1);
        }

        public void AssignGolemAction(ushort id)
        {
            List<PlantState> plants = gardenArea.wiltedPlantList;//gardenManager.GetPlants(gardenArea.areaIndex);
            if(plants.Count != 0)
            {
                //Debug.Log("Watering Task");
                golemManager.UpdateGolemTask(id);
            }

        }

        public void GolemDoAction(int value)
        {
            List<PlantState> plants = gardenArea.wiltedPlantList;
            int wiltedIndex = 0;
            bool fireEvent = false;

            if(returnFromPause)
            {
                GolemDoActionOnReturn(value);
                returnFromPause = false;
            } else
            {
                foreach (GolemState golem in golemManager.golemStateTable)
                {
                    // Add a check for Golem Happiness here once we get this sorted out
                    if ((ushort)golem.golemID != 0 && golem.assignedWatering)
                    {
                        if (golem.happiness > 0 && wiltedIndex < plants.Count)
                        {
                            //Debug.Log("Assigned Cell: " + plants[wiltedIndex].cell);
                            fireEvent = true;
                            golemManager.UpdateTaskCell((ushort)golem.golemID, plants[wiltedIndex].cell);
                            wiltedIndex++;
                        }
                        else
                        {
                            golemManager.UpdateGolemTask((ushort)golem.golemID);
                        }
                    }
                }

                if (fireEvent) EventManager.instance.HandleEVENT_GOLEM_DO_TASK(happinessChangeValue);
            }      
        }

        public void GolemDoActionOnReturn(int value)
        {
            List<PlantState> plants = gardenArea.wiltedPlantList;
            int numGolemsWatering = 0;

            //Debug.Log("Number of plants in wiltedPlantList on return: " + plants.Count);

            foreach (GolemState golem in golemManager.golemStateTable)
            {
                if (golem.assignedWatering && plants.Count > 0)
                {
                    numGolemsWatering++;
                    EventManager.instance.HandleEVENT_GOLEM_HAPPINESS_UPDATE((ushort)golem.golemID, (happinessChangeValue * value));
                    AssignGolemAction((ushort)golem.golemID);
                }
            }

            for (int i = 0; i < (value * numGolemsWatering); i++)
            {
                if (plants.Count > 0)
                {
                    gardenArea.OnReturnWaterPlant(plants[0].cell);
                }
            }    

        }

        Coroutine inputState;
        public void EnterDragGolem(GameObject golemObject)
        {
            inputState = gardenArea.StartCoroutine(GolemDragHandler(golemObject));
        }

          /// <summary>
        /// TODO: Dont calculate per frame
        /// </summary>
        /// <param name="gardenArea"></param>
        /// <returns></returns>
        IEnumerator GolemDragHandler(GameObject golemObject)
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(validCheckTime);

            Vector3 destination = gardenArea.lastSelectedPosition;
            destination.z = 0;

            golemObject.transform.position = destination;

            GolemController golemController = golemObject.GetComponent<GolemController>();
            GameObject golemRig = golemController.getActiveRig();
            bool isValid = false;

            do
            {
                destination = gardenArea.lastDraggedPosition;
                destination.z = 0;

                golemObject.transform.position = destination;

                Physics.SyncTransforms();

                isValid = CheckValidState(golemRig.GetComponentInChildren<Collider>());
                //print("Valid to drag: " + isValid);

                foreach (SpriteRenderer renderer in golemRig.GetComponentsInChildren<SpriteRenderer>())
                {
                    renderer.color = isValid ? Color.white : invalidColor;
                }

                yield return null;

            } while (golemObject && pointerState.phase != PointerState.Phase.NONE);

            foreach (SpriteRenderer renderer in golemRig.GetComponentsInChildren<SpriteRenderer>())
            {
                renderer.color = Color.white;
            }
            golemController.ExitDragAttempt(isValid);
        }
        public bool CheckValidState(Collider golemCollider)
        {
            Vector3Int tileBlockSize = Vector3Int.one;
            tileBlockSize.x = Mathf.CeilToInt(golemCollider.bounds.size.x);
            tileBlockSize.y = Mathf.CeilToInt(golemCollider.bounds.size.y); 

            Vector3Int tileBlockOrigin = Vector3Int.zero;
            tileBlockOrigin = gardenArea.tilemap.WorldToCell(golemCollider.transform.position);
            tileBlockOrigin.x -= tileBlockSize.x / 2;

            BoundsInt colliderBounds = new BoundsInt(tileBlockOrigin, tileBlockSize);

            TileBase[] m_tileBlock = gardenArea.tilemap.GetTilesBlock(colliderBounds);

            foreach (TileBase tileBase in m_tileBlock)
            {
                if (!tileStore[tileBase].pathable)
                    return false;
            }

            return true;
        }
    }
}
