using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Pathfinding;
using Core.RNG;

namespace GrandmaGreen.Entities {
    [CreateAssetMenu(menuName = "GrandmaGreen/Entities/Golem Manager")]
    public class GolemManager : ScriptableObject
    {
        public GameObject tulip;
        public List<int> golemList; //golem id

        public void Initialize()
        {
            //Subscribe
            EventManager.instance.EVENT_GOLEM_SPAWN += OnGolemSpawn;
        }

        /// <summary>
        /// Spawn Golem when harvest
        /// </summary>
        public void OnGolemSpawn(int id, Vector3 pos) {
            Debug.Log("A Golem is spawned" + pos.ToString());
            
            // if golem existed, not spawn
            if (golemList.Contains(id)) return;

            GameObject newGolemParent = new GameObject("newGolemParent_"+id);
            newGolemParent.AddComponent<SplineContainer>();
            
            // Instantiate at position and zero rotation.
            GameObject newGolem = Instantiate(tulip, pos, Quaternion.identity);
            newGolem.transform.rotation = Quaternion.Euler(-45,0,0);
            newGolem.transform.SetParent(newGolemParent.transform);
            newGolem.GetComponent<SplineFollow>().target = newGolemParent.GetComponent<SplineContainer>();

            // Add golem to the table
            golemList.Add(id);
        }

        public void OnDestroy() {
            //Unsubscribe
            EventManager.instance.EVENT_GOLEM_SPAWN -= OnGolemSpawn;
        }
    }
}
