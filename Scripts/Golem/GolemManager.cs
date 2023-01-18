using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Pathfinding;
using Core.RNG;
using GrandmaGreen.Collections;
using GrandmaGreen.Garden;

namespace GrandmaGreen.Entities 
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Entities/GolemManager")]
    public class GolemManager : ScriptableObject
    {
        public List<GameObject> golemPrefabs;
        //public Dictionary<ushort, GameObject> golemObjectTable; //active golems
        public GolemState[] golemStateTable;
        public EntityController playerController;

        public void Initialize()
        {
            //Subscribe
            EventManager.instance.EVENT_GOLEM_SPAWN += OnGolemSpawn;
            EventManager.instance.EVENT_GOLEM_HAPPINESS_UPDATE += OnGolemHappinessChanged;
            golemStateTable = new GolemState[10];
        }

        public GolemState CreateGolem(ushort id) {
            return new GolemState {
                golemID = (CharacterId)id,
                happiness = 50,
                isSpawned = true,
                isMature = false,
                isTravelling = false,
                assignedWatering = false,
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

            if (golemPrefabs.Count < 1) 
            {
                Debug.Log("Golem Manager: no prefebs availalbe");
                return;
            }

            Debug.Log("A Golem is spawned" + pos.ToString());
            GameObject newGolemParent = new GameObject("GolemParent_"+id);
            newGolemParent.AddComponent<SplineContainer>();
            
            // Instantiate at position and zero rotation.
            GameObject newGolem = Instantiate(golemPrefabs[0], pos, Quaternion.identity);
            newGolem.transform.rotation = Quaternion.Euler(-45,0,0);
            newGolem.transform.SetParent(newGolemParent.transform);
            newGolem.GetComponent<SplineFollow>().target = newGolemParent.GetComponent<SplineContainer>();

            // Register Manager
            newGolem.GetComponent<GolemController>().RegisterManager(this);

            // Add golem to the table
            golemStateTable[tableIndex] = CreateGolem(id);
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

            golemStateTable[index].assignedWatering = !golemStateTable[index].assignedWatering;
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
        }

        public void OnDestroy() {
            //Unsubscribe
            EventManager.instance.EVENT_GOLEM_SPAWN -= OnGolemSpawn;
            EventManager.instance.EVENT_GOLEM_HAPPINESS_UPDATE -= OnGolemHappinessChanged;
        }

        #region Utility
        private int offsetId(ushort id) {return (int)id - 5000;}
        private bool isIdValid(int index) { 
            return index < golemStateTable.Length && index >= 0;
        }
        #endregion
    }
}