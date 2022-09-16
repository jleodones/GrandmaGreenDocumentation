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

        // Invetory Event and Handler
        public event Action<string,int> EVENT_INVENTORY_ADD;
        public event Action<string,int> EVENT_INVENTORY_REMOVE;
        public event Action<string,int> EVENT_INVENTORY_UPDATE;
        
        public void HandleEVENT_INVENTORY_ADD(string id, int num) {EVENT_INVENTORY_ADD?.Invoke(id, num);}
        public void HandleEVENT_INVENTORY_REMOVE(string id, int num) {EVENT_INVENTORY_REMOVE?.Invoke(id, num);}
        public void HandleEVENT_INVENTORY_UPDATE(string id, int num) {EVENT_INVENTORY_UPDATE?.Invoke(id, num);}

        // Golem's Event and Handler
        public event Action<int> EVENT_GOLEM_SPAWN;

        public void HandleEVENT_GOLEM_SPAWN(int id) {EVENT_GOLEM_SPAWN?.Invoke(id);}

    }    
}

