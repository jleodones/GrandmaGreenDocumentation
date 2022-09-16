using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen {
    public class GolemSpawner : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //Subscribe
            EventManager.instance.EVENT_GOLEM_SPAWN += OnGolemSpawn;
        }

        public void OnGolemSpawn(int id) {
            Debug.Log("Flower " + id + " is harvasted" );
            Debug.Log("A Golem is spawned somewhere");
        }

        private void OnDestroy() {
            //Unsubscribe
            EventManager.instance.EVENT_GOLEM_SPAWN -= OnGolemSpawn;
        }
}
}
