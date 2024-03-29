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
        
        #region Save events and handlers.

        public event Action EVENT_MANUAL_SAVE;
        public event Action EVENT_DELETE_SAVE;
        public event Action EVENT_SYNC_AUTOSAVE;

        public void HandleEVENT_MANUAL_SAVE()
        {
            EVENT_MANUAL_SAVE?.Invoke();
        }

        public void HandleEVENT_DELETE_SAVE()
        {
            EVENT_DELETE_SAVE?.Invoke();
        }

        public void HandleEVENT_SYNC_AUTOSAVE()
        {
            EVENT_SYNC_AUTOSAVE?.Invoke();
        }
        
        #endregion

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
        public event Func<ushort, Genotype, int> EVENT_INVENTORY_GET_SEED_COUNT;
        
        public event Action<ushort> EVENT_INVENTORY_ADD_TOOL;
        public event Action<ushort> EVENT_INVENTORY_REMOVE_TOOL;

        public event Action<ushort> EVENT_INVENTORY_ADD_DECOR;
        public event Action<ushort> EVENT_INVENTORY_REMOVE_DECOR;

        public event Action<int> EVENT_INVENTORY_ADD_MONEY;
        public event Action<int> EVENT_INVENTORY_REMOVE_MONEY;
        public event Func<int> EVENT_INVENTORY_GET_MONEY;

        public event Action<ushort, Genotype> EVENT_SUBMIT_PLANT;
      
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

        public int? HandleEVENT_INVENTORY_GET_SEED_COUNT(ushort id, Genotype genotype)
        { 
            return EVENT_INVENTORY_GET_SEED_COUNT?.Invoke(id, genotype);
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

        public void HandleEVENT_SUBMIT_PLANT(ushort id, Genotype genotype)
        {
            EVENT_SUBMIT_PLANT?.Invoke(id, genotype);
        }
        #endregion

        #region Characters
        public event Action<ushort, ushort> EVENT_CHA_CHANGE_EMO;
        public event Action<ushort, ushort, float> EVENT_CHA_CHANGE_EMO_INTIME;

        public void HandleEVENT_CHA_CHANGE_EMO(ushort CharID, ushort EmoID) 
        { 
            EVENT_CHA_CHANGE_EMO?.Invoke(CharID, EmoID); 
        }

        public void HandleEVENT_CHA_CHANGE_EMO_INTIME(ushort CharID, ushort EmoID, float time)
        {
            EVENT_CHA_CHANGE_EMO_INTIME?.Invoke(CharID, EmoID, time);
        }
        #endregion

        #region  Golem events and handlers.
        public event Action<ushort, Vector3> EVENT_GOLEM_SPAWN;
        public event Action<ushort, int> EVENT_GOLEM_HAPPINESS_UPDATE;
        public event Action<ushort> EVENT_GOLEM_EVOLVE;
        public event Action<Vector3> EVENT_GOLEM_GRANDMA_MOVE_TO;
        public event Action<GameObject> EVENT_GOLEM_DRAG;
        public event Action EVENT_GOLEM_RELEASE_SELECTED;
        public event Action<ushort> EVENT_ASSIGN_TASK;
        public event Action<int> EVENT_GOLEM_DO_TASK;
        public event Action EVENT_GOLEM_ENABLE;
        public event Action EVENT_GOLEM_DISABLE;

        public void HandleEVENT_GOLEM_DRAG(GameObject obj) { EVENT_GOLEM_DRAG?.Invoke(obj); }
        public void HandleEVENT_GOLEM_RELEASE_SELECTED() {EVENT_GOLEM_RELEASE_SELECTED?.Invoke();}
        public void HandleEVENT_GOLEM_SPAWN(ushort id, Vector3 pos) { EVENT_GOLEM_SPAWN?.Invoke(id, pos); }
        public void HandleEVENT_GOLEM_EVOLVE(ushort id) { EVENT_GOLEM_EVOLVE?.Invoke(id); }
        
        public void HandleEVENT_GOLEM_HAPPINESS_UPDATE(ushort id, int val) 
        {
            EVENT_GOLEM_HAPPINESS_UPDATE?.Invoke(id, val);
        }

        public void HandleEVENT_GOLEM_GRANDMA_MOVE_TO(Vector3 pos)
        {
            EVENT_GOLEM_GRANDMA_MOVE_TO?.Invoke(pos);
        }

        public void HandleEVENT_GOLEM_ASSIGN_TASK(ushort id) { EVENT_ASSIGN_TASK?.Invoke(id); }

        public void HandleEVENT_GOLEM_DO_TASK(int happinessValue) { EVENT_GOLEM_DO_TASK?.Invoke(happinessValue); }

        public void HandleEVENT_GOLEM_ENABLE() { EVENT_GOLEM_ENABLE?.Invoke(); }
        public void HandleEVENT_GOLEM_DISABLE() { EVENT_GOLEM_DISABLE?.Invoke(); }
        #endregion

        #region Plant growth event.
        public event Action<int, Vector3Int> EVENT_PLANT_UPDATE;
        public event Action<Vector3Int> EVENT_WATER_PLANT;

        public void HandleEVENT_PLANT_UPDATE(int areaIndex, Vector3Int cell)
        {
            EVENT_PLANT_UPDATE?.Invoke(areaIndex, cell);
        }

        public void HandleEVENT_WATER_PLANT(Vector3Int cell)
        {
            EVENT_WATER_PLANT?.Invoke(cell);
        }
        #endregion
        
        #region Customization
        public event Action<IInventoryItem> EVENT_UPDATE_PLANT_COLLECTIONS;

        public void HandleEVENT_UPDATE_PLANT_COLLECTIONS(IInventoryItem item)
        {
            EVENT_UPDATE_PLANT_COLLECTIONS.Invoke(item);
        }
        #endregion
       
        #region  Customization.
        public event Action<IInventoryItem> EVENT_INVENTORY_CUSTOMIZATION_START;
        public event Action EVENT_TOGGLE_CUSTOMIZATION_MODE;
        public event Action<bool> EVENT_CUSTOMIZATION_ATTEMPT;

        public void HandleEVENT_INVENTORY_CUSTOMIZATION_START(IInventoryItem item)
        {
            EVENT_INVENTORY_CUSTOMIZATION_START?.Invoke(item);
        }

        public void HandleEVENT_TOGGLE_CUSTOMIZATION_MODE()
        {
            EVENT_TOGGLE_CUSTOMIZATION_MODE?.Invoke();
        }

        public void HandleEVENT_CUSTOMIZATION_ATTEMPT(bool successful)
        {
            EVENT_CUSTOMIZATION_ATTEMPT?.Invoke(successful);
        }
        #endregion

    }
}

