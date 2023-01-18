using System.Collections;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using GrandmaGreen.Garden;

namespace GrandmaGreen.Entities
{
    public class GolemCrowdController : MonoBehaviour
    {
        [Header("Golem Management")]
        public GolemManager golemManager;

        [Header("Golem References")]
        public GardenAreaController gardenArea;

        public void Awake()
        {
            golemManager.Initialize();
            EventManager.instance.EVENT_ASSIGN_TASK += AssignGolemAction;
        }

        [Header("Debug Options")]
        public CharacterId theGolem = CharacterId.PumpkinGolem;
        public int value = 0;

        [Button(ButtonSizes.Medium)]
        public void UpdateGolemHappiness()
        {
            EventManager.instance.HandleEVENT_GOLEM_HAPPINESS_UPDATE((ushort)theGolem, value);
        }

        [Button(ButtonSizes.Medium)]
        public void SpawnGolem()
        {
            ushort id = (ushort)CharacterId.TulipGolem;
            Vector3 pos = new Vector3(0,0,0);
            golemManager.OnGolemSpawn(id, pos);
        }

        public void AssignGolemAction(ushort id)
        {
            List<PlantState> plants = gardenArea.wiltedPlantList;//gardenManager.GetPlants(gardenArea.areaIndex);
            if(plants.Count != 0)
            {
                //Debug.Log("Watering Task");
                //int randIndex = Random.Range(0, plants.Count - 1);
                //Debug.Log(plants[randIndex].cell);
                golemManager.UpdateGolemTask(id);
            }

        }

        public void GolemDoAction()
        {
            List<PlantState> plants = gardenArea.wiltedPlantList;
            foreach(GolemState golem in golemManager.golemStateTable)
            {
                if (golem.assignedWatering)
                {
                    int randIndex = Random.Range(0, plants.Count - 1);
                    int randIndex2 = randIndex + 1;
                    Debug.Log("Assigned Cell: " + plants[randIndex].cell);
                    //Debug.Log((ushort)golem.golemID);
                    //golemManager.UpdateTaskCell((ushort)golem.golemID, plants[randIndex].cell);
                    golemManager.UpdateTaskCell((ushort)CharacterId.PumpkinGolem, plants[randIndex].cell);
                    if(!(randIndex2 >= plants.Count)) golemManager.UpdateTaskCell((ushort)CharacterId.TulipGolem, plants[randIndex2].cell);
                    else golemManager.UpdateTaskCell((ushort)CharacterId.TulipGolem, plants[randIndex].cell);
                }
            }

            EventManager.instance.HandleEVENT_GOLEM_DO_TASK();
        }

        [Button(ButtonSizes.Medium)]
        public void WaterGolem()
        {
            GolemDoAction();
        }
    }
}
