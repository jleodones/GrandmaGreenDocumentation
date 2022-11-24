using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Pathfinding;
using Core.RNG;
using GrandmaGreen.Collections;

namespace GrandmaGreen.Entities 
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Entities/GolemManager")]
    public class GolemManager : ScriptableObject
    {
        public List<GameObject> golemPrefabs;
        //public Dictionary<ushort, GameObject> golemObjectTable; //active golems
        public GolemState[] golemStateTable;

        public void Initialize()
        {
            //Subscribe
            EventManager.instance.EVENT_GOLEM_SPAWN += OnGolemSpawn;
            golemStateTable = new GolemState[10];
        }

        public GolemState CreateGolem(ushort id) {
            return new GolemState {
                golemID = (CharacterId)id,
                happiness = 50,
                isSpawned = true,
                isMature = false,
                isTravelling = false,
            };
        }

        /// <summary>
        /// Spawn Golem when harvest
        /// </summary>
        public void OnGolemSpawn(ushort id, Vector3 pos) {
            
            // if golem existed, not spawn
            int tableIndex = (int)id - 5000;
            if (tableIndex >= golemStateTable.Length || 
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
            //newGolem.transform.rotation = Quaternion.Euler(-45,0,0);
            newGolem.transform.SetParent(newGolemParent.transform);
            newGolem.GetComponent<SplineFollow>().target = newGolemParent.GetComponent<SplineContainer>();

            // Add golem to the table
            golemStateTable[tableIndex] = CreateGolem(id);
        }

        public void OnDestroy() {
            //Unsubscribe
            EventManager.instance.EVENT_GOLEM_SPAWN -= OnGolemSpawn;
        }

        private int offsetId(ushort id) {return (int)id - 5000;}
    }
}
