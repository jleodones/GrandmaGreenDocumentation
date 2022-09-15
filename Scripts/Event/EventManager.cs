using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager instance;

        private void Awake() {
            if (instance == null) {
                instance = this;    
            } else {
                Destroy(this);
            }   
        }

        // Golem's Event
        public event Action<int> GOLEM_SPAWN;
        public void InvokeGolemSpawn(int id) { 
            GOLEM_SPAWN?.Invoke(id);
        }

    }    
}

