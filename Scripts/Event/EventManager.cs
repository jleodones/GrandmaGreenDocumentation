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

        #region  HUD events and handlers.
        public event Action EVENT_OPEN_HUD;
        public event Action EVENT_OPEN_HUD_ANIMATED;
        public event Action EVENT_CLOSE_HUD;
        public event Action EVENT_CLOSE_HUD_ANIMATED;
        public event Action EVENT_UPDATE_MONEY_DISPLAY;

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

        public void HandleEVENT_UPDATE_MONEY_DISPLAY()
        {
            EVENT_UPDATE_MONEY_DISPLAY?.Invoke();
        }
        #endregion

        #region Inventory related events and handlers.
        public event Action EVENT_INVENTORY_OPEN;
        
        public event Action<ushort, Genotype> EVENT_INVENTORY_ADD_PLANT;
        public event Action<ushort, Genotype> EVENT_INVENTORY_REMOVE_PLANT;
        
        public event Action<ushort, Genotype> EVENT_INVENTORY_ADD_SEED;
        public event Action<ushort, Genotype> EVENT_INVENTORY_REMOVE_SEED;
        
        public event Action<ushort> EVENT_INVENTORY_ADD_TOOL;
        public event Action<ushort> EVENT_INVENTORY_REMOVE_TOOL;

        public event Action<ushort> EVENT_INVENTORY_ADD_DECOR;
        public event Action<ushort> EVENT_INVENTORY_REMOVE_DECOR;

        /*
        public event Action<IInventoryItem, int> EVENT_INVENTORY_ADD_TOOL_OR_DECOR;
        public event Action<IInventoryItem, int> EVENT_INVENTORY_REMOVE_TOOL_OR_DECOR;

        public event Action<IInventoryItem, Genotype> EVENT_INVENTORY_ADD_PLANT_OR_SEED;
        public event Action<IInventoryItem, Genotype> EVENT_INVENTORY_REMOVE_PLANT_OR_SEED;
        */

        public event Action<int> EVENT_INVENTORY_ADD_MONEY;
        public event Action<int> EVENT_INVENTORY_REMOVE_MONEY;
        public event Func<int> EVENT_INVENTORY_GET_MONEY;
        #endregion

        #region  Customization.
        public event Action<IInventoryItem> EVENT_CUSTOMIZATION_START;
        public event Action<bool> EVENT_CUSTOMIZATION_END;

        public void HandleEVENT_INVENTORY_OPEN()
        {
            EVENT_INVENTORY_OPEN?.Invoke();
        }

        public void HandleEVENT_INVENTORY_ADD_PLANT(ushort id, Genotype genotype)
        {
            EVENT_INVENTORY_ADD_PLANT?.Invoke(id, genotype);
        }
        
        public void HandleEVENT_INVENTORY_REMOVE_PLANT(ushort id, Genotype genotype)
        {
            EVENT_INVENTORY_REMOVE_PLANT?.Invoke(id, genotype);
        }
        
        public void HandleEVENT_INVENTORY_ADD_SEED(ushort id, Genotype genotype)
        {
            EVENT_INVENTORY_ADD_SEED?.Invoke(id, genotype);
        }

        public void HandleEVENT_INVENTORY_REMOVE_SEED(ushort id, Genotype genotype)
        {
            EVENT_INVENTORY_REMOVE_SEED?.Invoke(id, genotype);
        }
        
        public void HandleEVENT_INVENTORY_ADD_TOOL(ushort id)
        {
            EVENT_INVENTORY_ADD_TOOL?.Invoke(id);
        }
        
        public void HandleEVENT_INVENTORY_REMOVE_TOOL(ushort id)
        {
            EVENT_INVENTORY_REMOVE_TOOL?.Invoke(id);
        }
        
        public void HandleEVENT_INVENTORY_ADD_DECOR(ushort id)
        {
            EVENT_INVENTORY_ADD_DECOR?.Invoke(id);
        }
        
        public void HandleEVENT_INVENTORY_REMOVE_DECOR(ushort id)
        {
            EVENT_INVENTORY_REMOVE_DECOR?.Invoke(id);
        }

        public void HandleEVENT_INVENTORY_ADD_MONEY(int money)
        {
            EVENT_INVENTORY_ADD_MONEY?.Invoke(money);
            HandleEVENT_UPDATE_MONEY_DISPLAY();
        }

        public void HandleEVENT_INVENTORY_REMOVE_MONEY(int money)
        {
            EVENT_INVENTORY_REMOVE_MONEY?.Invoke(money);
            HandleEVENT_UPDATE_MONEY_DISPLAY();
        }

        public int HandleEVENT_INVENTORY_GET_MONEY()
        {
            return EVENT_INVENTORY_GET_MONEY();
        }
        #endregion

        #region  Golem events and handlers.
        public event Action<ushort, Vector3> EVENT_GOLEM_SPAWN;

        public void HandleEVENT_GOLEM_SPAWN(ushort id, Vector3 pos) { EVENT_GOLEM_SPAWN?.Invoke(id, pos); }

        #endregion

        #region Plant growth event.
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

        #endregion
    }
}

