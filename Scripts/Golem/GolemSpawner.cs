using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Pathfinding;
using GrandmaGreen.Entities;

namespace GrandmaGreen {
    public class GolemSpawner : MonoBehaviour
    {
        public GameObject tulip;
        public EntityController AIController;

        // Start is called before the first frame update
        void Start()
        {
            //Subscribe
            EventManager.instance.EVENT_GOLEM_SPAWN += OnGolemSpawn;
        }

        public void OnGolemSpawn(int id, Vector3 pos) {
            Debug.Log("Flower " + id + " is harvasted" );
            Debug.Log("A Golem is spawned somewhere");

            GameObject newGolemParent = new GameObject("newGolemParent_"+id);
            newGolemParent.AddComponent<SplineContainer>();
            
            // Instantiate at position and zero rotation.
            GameObject newGolem = Instantiate(tulip, pos, Quaternion.identity);
            newGolem.transform.SetParent(newGolemParent.transform);
            newGolem.GetComponent<SplineFollow>().target = newGolemParent.GetComponent<SplineContainer>();

            // Create Entity Controller on fly
            EntityController controller = Instantiate(AIController);
            newGolem.GetComponent<GameEntity>().controller = controller;
        }

        private void OnDestroy() {
            //Unsubscribe
            EventManager.instance.EVENT_GOLEM_SPAWN -= OnGolemSpawn;
        }
}
}
