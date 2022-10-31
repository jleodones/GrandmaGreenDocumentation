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
        
        // HUD events and handlers.
        public event Action EVENT_OPEN_HUD;
        public event Action EVENT_CLOSE_HUD;

        public void HandleEVENT_OPEN_HUD()
        {
            EVENT_OPEN_HUD?.Invoke();
        }

        public void HandleEVENT_CLOSE_HUD()
        {
            EVENT_CLOSE_HUD?.Invoke();
        }

        // Inventory related events and handlers.
        public event Action EVENT_INVENTORY_OPEN;
        public event Action<IInventoryItem,int> EVENT_INVENTORY_ADD;
        public event Action<IInventoryItem,int> EVENT_INVENTORY_REMOVE;

        public void HandleEVENT_INVENTORY_OPEN()
        {
            EVENT_INVENTORY_OPEN?.Invoke();
        }
        
        public void HandleEVENT_INVENTORY_ADD(IInventoryItem item, int num)
        {
            EVENT_INVENTORY_ADD?.Invoke(item, num);
        }
        public void HandleEVENT_INVENTORY_REMOVE(IInventoryItem item, int num) {EVENT_INVENTORY_REMOVE?.Invoke(item, num);}

        // Golem events and handlers.
        public event Action<int, Vector3> EVENT_GOLEM_SPAWN;

        public void HandleEVENT_GOLEM_SPAWN(int id, Vector3 pos) {EVENT_GOLEM_SPAWN?.Invoke(id, pos);}

        // Plant growth event.
        public event Action<int, Vector3Int> EVENT_PLANT_UPDATE;

        public void HandleEVENT_PLANT_UPDATE(int areaIndex, Vector3Int cell)
        {
            EVENT_PLANT_UPDATE?.Invoke(areaIndex, cell);
        }
    }    
}

