using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GrandmaGreen.Collections;

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

        // Inventory Event and Handlers.
        public event Action<IInventoryItem,int> EVENT_INVENTORY_ADD;
        public event Action<IInventoryItem,int> EVENT_INVENTORY_REMOVE;
        public event Action<IInventoryItem> EVENT_INVENTORY_UPDATE;

        public void HandleEVENT_INVENTORY_ADD(IInventoryItem item, int num)
        {
            EVENT_INVENTORY_ADD?.Invoke(item, num);
        }
        public void HandleEVENT_INVENTORY_REMOVE(IInventoryItem item, int num) {EVENT_INVENTORY_REMOVE?.Invoke(item, num);}
        public void HandleEVENT_INVENTORY_UPDATE(IInventoryItem item) {EVENT_INVENTORY_UPDATE?.Invoke(item);}

        // Golem's Event and Handler
        public event Action<int, Vector3> EVENT_GOLEM_SPAWN;

        public void HandleEVENT_GOLEM_SPAWN(int id, Vector3 pos) {EVENT_GOLEM_SPAWN?.Invoke(id, pos);}

        // Plant growth Event
        public event Action<int, Vector3Int> EVENT_PLANT_UPDATE;

        public void HandleEVENT_PLANT_UPDATE(int areaIndex, Vector3Int cell)
        {
            EVENT_PLANT_UPDATE?.Invoke(areaIndex, cell);
        }
    }    
}

