using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Tilemaps;
using Pathfinding;
using Core.RNG;
using GrandmaGreen.Collections;
using GrandmaGreen.Dialogue;
using GrandmaGreen.Garden;
using GrandmaGreen.SaveSystem;
using GrandmaGreen.UI.Golems;
using Newtonsoft.Json;

namespace GrandmaGreen.Entities 
{
    using Timer = TimeLayer.TimeLayer;
    
    [CreateAssetMenu(menuName = "GrandmaGreen/Entities/GolemManager")]
    public class GolemManager : ObjectSaver
    {
        [JsonIgnore]
        public List<GameObject> golemPrefabs;
        //public Dictionary<ushort, GameObject> golemObjectTable; //active golems
        [JsonIgnore]
        public int GolemNum = 10;

        [JsonIgnore]
        public EntityController playerController;

        [JsonIgnore]
        public Timer golemWorkTimer;

        [JsonIgnore]
        public int golemHappinessThreshold = 30;

        [JsonIgnore]
        public GolemState[] golemStateTable;

        [JsonIgnore]
        public Tilemap tiles;

        [JsonIgnore]
        public Dictionary<ushort, GameObject> activeGolems;

        [JsonIgnore]
        public List<GolemStorylineProgressionRequirement> golemStorylineProgressionRequirements;

        public bool IsSpawnDemoGolem = true;

        [JsonIgnore]
        public CameraZoom cameraZoom;
        
        public void Initialize()
        {
            //Subscribe
            EventManager.instance.EVENT_GOLEM_SPAWN += OnGolemSpawn;
            EventManager.instance.EVENT_GOLEM_HAPPINESS_UPDATE += OnGolemHappinessChanged;
            EventManager.instance.EVENT_SYNC_AUTOSAVE += SaveGolemData;
            EventManager.instance.EVENT_DELETE_SAVE += TriggerDeleteData;

            if (componentStores.Count == 0)
            {
                CreateNewStore(typeof(GolemState));
            }

            activeGolems = new Dictionary<ushort, GameObject>();
        }

        public GolemState CreateGolem(ushort id) {
            return new GolemState {
                golemID = (CharacterId)id,
                happiness = 0,
                isSpawned = true,
                isMature = false,
                isTravelling = false,
                assignedWatering = false,
                storylineProgression =  0,
            };
        }

        /// <summary>
        /// Load Golem from save file
        /// </summary>
        public void LoadGolemToScene(int tableIndex, GolemState state, Vector3 pos)
        {
            GameObject newGolemParent = new GameObject("GolemParent_"+state.golemID);
            newGolemParent.AddComponent<SplineContainer>();
            
            // Instantiate at position and zero rotation.
            GameObject newGolem = Instantiate(golemPrefabs[tableIndex], pos, Quaternion.identity);
            newGolem.transform.rotation = Quaternion.Euler(-45,0,0);
            newGolem.transform.SetParent(newGolemParent.transform);
            newGolem.GetComponent<SplineFollow>().target = newGolemParent.GetComponent<SplineContainer>();

            // Register Manager
            newGolem.GetComponent<GolemController>().RegisterManager(this);

            if (state.isMature) newGolem.GetComponent<GolemController>().UpdateMatureState(true);
            
            // Register golem with splash screen and evolution UI. LOL.
            if (!activeGolems.TryAdd((ushort)(newGolem.GetComponent<GolemController>().id),
                    newGolem))
            {
                // If it already exists, replace the old null data.
                activeGolems[(ushort)newGolem.GetComponent<GolemController>().id] = newGolem;
            };
        }

        /// <summary>
        /// Spawn Golem when harvest
        /// </summary>
        public void OnGolemSpawn(ushort id, Vector3 pos) {
        
            // if golem existed, not spawn
            int tableIndex = offsetId(id);
            if (!isIdValid(tableIndex) || 
                golemStateTable[tableIndex].golemID == (CharacterId)id)
            {
                Debug.Log("Golem Existed or wrong id");
                return;
            }

            if (golemPrefabs.Count < tableIndex) 
            {
                Debug.Log("Golem Manager: no prefabs available.");
                return;
            }

            Debug.Log("A Golem is spawned" + pos.ToString());
            
            // Add golem to the table
            golemStateTable[tableIndex] = CreateGolem(id);

            LoadGolemToScene(tableIndex,golemStateTable[tableIndex], pos);
        }

        public void OnGolemHappinessChanged(ushort id, int val) {
            int index = offsetId(id);
            if (!isIdValid(index)) {
                Debug.LogError("Golem id wrong");
                return;
            }

            Debug.Log("Golem Happiness Changed " + val);
            int happiness = golemStateTable[index].happiness;
            happiness += val;
            happiness = happiness < 0? 0 : happiness;

            // check evolve
            if (happiness >= 100 && !golemStateTable[index].isMature) {
                EventManager.instance.HandleEVENT_GOLEM_EVOLVE(id);
                golemStateTable[index].isMature = true;
                happiness = 50;
            }

            golemStateTable[index].happiness = happiness;
            
            // Update storyline progression.
            bool maturity = golemStateTable[index].isMature;
            int progress = golemStateTable[index].storylineProgression;
            
            foreach (GolemStorylineProgressionRequirement r in golemStorylineProgressionRequirements)
            {
                if (r.VerifyAchievement(happiness, maturity, progress))
                {
                    golemStateTable[index].storylineProgression++;
                    activeGolems[id].transform.Find("Golem Menu").transform.Find("Dialogueable")
                        .GetComponent<Dialogueable>().TriggerDialogueByNode("Story_" + (progress + 1).ToString().TrimStart('0'));
                }
            }
        }

        public Vector3 GetPlayerPos()
        {
            return playerController.GetEntityPos();
        }

        public void UpdateGolemTask(ushort id)
        {
            int index = offsetId(id);
            if (!isIdValid(index))
            {
                Debug.LogError("Golem id wrong");
                return;
            }

            if(golemStateTable[index].happiness >= golemHappinessThreshold && golemStateTable[index].isMature) golemStateTable[index].assignedWatering = true;
            else golemStateTable[index].assignedWatering = false;
        }

        public void UpdateTaskCell(ushort id, Vector3Int cell)
        {
            int index = offsetId(id);
            if (!isIdValid(index))
            {
                Debug.LogError("Golem id wrong");
                return;
            }

            golemStateTable[index].assignedCell = cell;
            Vector3 cellInWorldSpace = tiles.CellToWorld(cell);
            float[] worldSpaceArray = new float[] {cellInWorldSpace.x, cellInWorldSpace.y, cellInWorldSpace.z};

            golemStateTable[index].assignedCellInWorldSpace = worldSpaceArray;
        }

        public void OnDestroy() {
            //Unsubscribe
            EventManager.instance.EVENT_GOLEM_SPAWN -= OnGolemSpawn;
            EventManager.instance.EVENT_GOLEM_HAPPINESS_UPDATE -= OnGolemHappinessChanged;
            EventManager.instance.EVENT_SYNC_AUTOSAVE -= SaveGolemData;
            EventManager.instance.EVENT_DELETE_SAVE -= TriggerDeleteData;
        }

        #region Saving
        public void LoadGolemData()
        {
            golemStateTable = new GolemState[10];

            GolemState newState = new GolemState();

            for (int index = 0; index < golemStateTable.Length; index++)
            {
                //int index = Array.IndexOf(golemStateTable, golem);
                Debug.LogWarning("requesting golem data" + index);
                if(RequestData<GolemState>(index,ref newState))
                {
                    golemStateTable[index] = newState;
                }
                else
                {
                    Debug.Log("adding golem data" + index);
                    AddComponent<GolemState>(index, new GolemState());
                }

                if(golemStateTable[index].isSpawned == true)
                {
                    Debug.Log("Golem " + index + " loaded");
                    LoadGolemToScene(index, golemStateTable[index], new Vector3(0,0,0));
                    IsSpawnDemoGolem = false;
                } 
            }

            // Hardcode for demo purpose
            if (IsSpawnDemoGolem) 
                EventManager.instance.HandleEVENT_GOLEM_SPAWN((ushort)CharacterId.Pumpkin, new Vector3Int(0,0,0));
        }

        public void SaveGolemData()
        {
            for (int index = 0; index < golemStateTable.Length; index++)
            {
                if (!UpdateValue<GolemState>(index, golemStateTable[index]))
                    Debug.Log("Could not save Golem Data!");
            }
        }

        public void TriggerDeleteData()
        {
            golemStateTable = new GolemState[10];
            activeGolems.Clear();
        }
        #endregion

        #region Utility
        private int offsetId(ushort id) {return (int)id - 5000;}
        private bool isIdValid(int index) { 
            return index < golemStateTable.Length && index >= 0;
        }
        #endregion
    }
}