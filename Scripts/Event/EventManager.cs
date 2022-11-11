using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GrandmaGreen.Collections;
using GrandmaGreen.Garden;

namespace GrandmaGreen
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        // HUD events and handlers.
        public event Action EVENT_OPEN_HUD;
        public event Action EVENT_OPEN_HUD_ANIMATED;
        public event Action EVENT_CLOSE_HUD;
        public event Action EVENT_CLOSE_HUD_ANIMATED;

        public void HandleEVENT_OPEN_HUD()
        {
            EVENT_OPEN_HUD?.Invoke();
        }

        public void HandleEVENT_CLOSE_HUD()
        {
            EVENT_CLOSE_HUD?.Invoke();
        }
        public void HandleEVENT_OPEN_HUD_ANIMATED()
        {
            EVENT_OPEN_HUD_ANIMATED?.Invoke();
        }

        public void HandleEVENT_CLOSE_HUD_ANIMATED()
        {
            EVENT_CLOSE_HUD_ANIMATED?.Invoke();
        }

        // Inventory related events and handlers.
        public event Action EVENT_INVENTORY_OPEN;
        public event Action<IInventoryItem, int> EVENT_INVENTORY_ADD_TOOL_OR_DECOR;
        public event Action<IInventoryItem, int> EVENT_INVENTORY_REMOVE_TOOL_OR_DECOR;

        public event Action<IInventoryItem, Genotype> EVENT_INVENTORY_ADD_PLANT_OR_SEED;
        public event Action<IInventoryItem, Genotype> EVENT_INVENTORY_REMOVE_PLANT_OR_SEED;

        public event Action<IInventoryItem> EVENT_CUSTOMIZATION_START;
        public event Action<bool> EVENT_CUSTOMIZATION_END;

        public void HandleEVENT_INVENTORY_OPEN()
        {
            EVENT_INVENTORY_OPEN?.Invoke();
        }

        public void HandleEVENT_INVENTORY_ADD_TOOL_OR_DECOR(IInventoryItem item, int num)
        {
            EVENT_INVENTORY_ADD_TOOL_OR_DECOR?.Invoke(item, num);
        }

        public void HandleEVENT_INVENTORY_REMOVE_TOOL_OR_DECOR(IInventoryItem item, int num)
        {
            EVENT_INVENTORY_REMOVE_TOOL_OR_DECOR?.Invoke(item, num);
        }

        public void HandleEVENT_INVENTORY_ADD_PLANT_OR_SEED(IInventoryItem item, Genotype genotype)
        {
            EVENT_INVENTORY_ADD_PLANT_OR_SEED?.Invoke(item, genotype);
        }

        public void HandleEVENT_INVENTORY_REMOVE_PLANT_OR_SEED(IInventoryItem item, Genotype genotype)
        {
            EVENT_INVENTORY_REMOVE_PLANT_OR_SEED?.Invoke(item, genotype);
        }

        // Golem events and handlers.
        public event Action<int, Vector3> EVENT_GOLEM_SPAWN;

        public void HandleEVENT_GOLEM_SPAWN(int id, Vector3 pos) { EVENT_GOLEM_SPAWN?.Invoke(id, pos); }

        // Plant growth event.
        public event Action<int, Vector3Int> EVENT_PLANT_UPDATE;

        public void HandleEVENT_PLANT_UPDATE(int areaIndex, Vector3Int cell)
        {
            EVENT_PLANT_UPDATE?.Invoke(areaIndex, cell);
        }

        public void HandleEVENT_CUSTOMIZATION_START(IInventoryItem item)
        {
            EVENT_CUSTOMIZATION_START?.Invoke(item);
        }

        public void HandleEVENT_CUSTOMIZATION_END(bool successful)
        {
            EVENT_CUSTOMIZATION_END?.Invoke(successful);
        }
    }
}

