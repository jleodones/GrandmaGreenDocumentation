using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Pathfinding;
using Core.RNG;
using GrandmaGreen.Collections;

namespace GrandmaGreen.Entities {
    [CreateAssetMenu(menuName = "GrandmaGreen/Entities/GolemManager")]
    public class GolemManager : ScriptableObject
    {
        public List<GameObject> golemPrefabs;
        public Dictionary<int, GameObject> golemObjectTable; //active golems
        public GolemState[] golemStateTable;

        public void Initialize()
        {
            //Subscribe
            EventManager.instance.EVENT_GOLEM_SPAWN += OnGolemSpawn;
        }

        public GolemState CreateGolem(CharacterId id) {
            return new GolemState {
                golemID = id,
                happiness = 50,
                isSpawned = true,
                isMature = false,
                isTravelling = false,
            };
        }

        /// <summary>
        /// Spawn Golem when harvest
        /// </summary>
        public void OnGolemSpawn(int id, Vector3 pos) {
            Debug.Log("A Golem is spawned" + pos.ToString());
            
            // if golem existed, not spawn
            if (golemObjectTable.ContainsKey(id)) return;

            GameObject newGolemParent = new GameObject("newGolemParent_"+id);
            newGolemParent.AddComponent<SplineContainer>();
            
            // Instantiate at position and zero rotation.
            GameObject newGolem = Instantiate(golemPrefabs[0], pos, Quaternion.identity);
            newGolem.transform.rotation = Quaternion.Euler(-45,0,0);
            newGolem.transform.SetParent(newGolemParent.transform);
            newGolem.GetComponent<SplineFollow>().target = newGolemParent.GetComponent<SplineContainer>();

            // Add golem to the table
            golemObjectTable[id] = newGolem;
        }

        public void OnDestroy() {
            //Unsubscribe
            EventManager.instance.EVENT_GOLEM_SPAWN -= OnGolemSpawn;
        }
    }
}
