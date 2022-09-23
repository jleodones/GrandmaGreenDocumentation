using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Pathfinding;

namespace GrandmaGreen {
    public class GolemSpawner : MonoBehaviour
    {
        public GameObject tulip;
        // Start is called before the first frame update
        void Start()
        {
            //Subscribe
            EventManager.instance.EVENT_GOLEM_SPAWN += OnGolemSpawn;
        }

        public void OnGolemSpawn(int id, Vector3 pos) {
            Debug.Log("Flower " + id + " is harvasted" );
            Debug.Log("A Golem is spawned somewhere");

            // Instantiate at position (0, 0, 0) and zero rotation.
            GameObject newGolem = new GameObject("NewGolemParent_"+id);
            newGolem.transform.SetParent(tulip.transform);
            newGolem.AddComponent<SplineContainer>();
            tulip.GetComponent<SplineFollow>().target = newGolem.GetComponent<SplineContainer>();
            Instantiate(tulip, pos, Quaternion.identity);
        }

        private void OnDestroy() {
            //Unsubscribe
            EventManager.instance.EVENT_GOLEM_SPAWN -= OnGolemSpawn;
        }
}
}
