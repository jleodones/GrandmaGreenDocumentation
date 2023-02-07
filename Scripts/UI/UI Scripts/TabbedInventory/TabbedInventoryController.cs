// This script defines the tab selection logic.
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using GrandmaGreen.SaveSystem;
using GrandmaGreen.Collections;
using GrandmaGreen.Garden;
using SpookuleleAudio;

namespace GrandmaGreen.UI.Collections
{
    public class TabbedInventoryController
    {
        // Owning tabbed inventory UI display.
        private TabbedInventory m_parent;
        
        // Inventory data.
        private ObjectSaver m_inventoryData;

        // Inventory width varibles for show/hide
        private Length inventoryWidth = (Length)(.45 * Screen.width);
        // Customization variables
        private VisualElement threshold; 
        private Button draggable;
        private Vector3 draggableStartPos;
        private IInventoryItem m_inventoryItemId;

        /// <summary>
        /// The TabbedInventoryController is attached to the TabbedInventory UI. It registers and controlls tab switching, as well as loading old and new items into the inventory.
        /// </summary>
        public TabbedInventoryController(TabbedInventory parent, ObjectSaver inventoryData)
        {
            m_parent = parent;
            m_inventoryData = inventoryData;
        }
        
                /// Everything here needs to be moved to the InventoryUIController later.
        public void InventoryAddMoney(int money)
        {
            // Adds to the singular money component store (int).
            int currentMoney = money;
            if (m_inventoryData.RequestData<int>(0, ref currentMoney))
            {
                currentMoney += money;
                m_inventoryData.UpdateValue<int>(0, currentMoney);
            }
        }
        public void InventoryRemoveMoney(int money)
        {
            int currentMoney = money;
            if (m_inventoryData.RequestData<int>(0, ref currentMoney))
            {
                currentMoney -= money;
                if (currentMoney < 0)
                {
                    currentMoney = 0;
                }
                m_inventoryData.UpdateValue<int>(0, currentMoney);
            }
        }

        public int InventoryGetMoney()
        {
            int currentMoney = 0;
            m_inventoryData.RequestData<int>(0, ref currentMoney);
            return currentMoney;
        }

        public void InventoryAddSeed(ushort id, Genotype genotype)
        {
            Seed s = new Seed(id, CollectionsSO.LoadedInstance.GetItem(id).name, genotype);

            if (m_inventoryData.RequestData(-1, ref s))
            {
                s.quantity += 1;
            }
            m_inventoryData.UpdateValue(-1, s);
                    
            // Update the UI system.
            m_parent.RebuildJar(s);
            
            // Call collections for potential update.
            EventManager.instance.HandleEVENT_UPDATE_PLANT_COLLECTIONS(s);
        }

        public void InventoryRemoveSeed(ushort id, Genotype genotype)
        {
            Seed s = new Seed(id, CollectionsSO.LoadedInstance.GetItem(id).name, genotype);

            if (m_inventoryData.RequestData(-1, ref s))
            {
                s.quantity -= 1;

                if (s.quantity <= 0)
                {
                    m_inventoryData.RemoveComponent<Seed>(-1, s);
                }
                else
                {
                    m_inventoryData.UpdateValue<Seed>(-1, s);
                }
                        
                // Then update the UI system.
                m_parent.RebuildJar(s);
            }
        }

        public int InventoryGetSeedCount(ushort id, Genotype genotype)
        {
            Seed s = new Seed(id, CollectionsSO.LoadedInstance.GetItem(id).name, genotype);

            if (m_inventoryData.RequestData(-1, ref s))
            {
                return s.quantity;
            }
            else return 0;
        }

        public void InventoryAddPlant(ushort id, Genotype genotype)
        {
            Plant p = new Plant(id, CollectionsSO.LoadedInstance.GetItem(id).name, genotype);
            
            if (m_inventoryData.RequestData(-1, ref p))
            {
                p.quantity += 1;
            }
            else
            {
                m_inventoryData.AddComponent(-1, p);
            }
            
            m_parent.RebuildJar(p);
            
            // Call collections for potential update.
            EventManager.instance.HandleEVENT_UPDATE_PLANT_COLLECTIONS(p);
        }
        
        public void InventoryRemovePlant(ushort id, Genotype genotype)
        {
            Plant p = new Plant(id, CollectionsSO.LoadedInstance.GetItem(id).name, genotype);
            
            if (m_inventoryData.RequestData<Plant>(-1, ref p))
            {
                p.quantity -= 1;

                if (p.quantity <= 0)
                {
                    m_inventoryData.RemoveComponent<Plant>(-1, p);
                }
                else
                {
                    m_inventoryData.UpdateValue<Plant>(-1, p);
                }
                m_parent.RebuildJar(p);
            }
        }

        public void InventoryAddTool(ushort id)
        {
            Tool t = new Tool(id, CollectionsSO.LoadedInstance.GetItem(id).name);
            
            if (m_inventoryData.RequestData(-1, ref t))
            {
                t.quantity += 1;
            }
            m_inventoryData.UpdateValue(-1, t);
            m_parent.RebuildJar(t);
        }

        public void InventoryRemoveTool(ushort id)
        {
            Tool t = new Tool(id, CollectionsSO.LoadedInstance.GetItem(id).name);

            if (m_inventoryData.RequestData(-1, ref t))
            {
                t.quantity -= 1;
                
                if (t.quantity <= 0)
                {
                    m_inventoryData.RemoveComponent(-1, t);
                }
                else
                {
                    m_inventoryData.UpdateValue(-1, t);
                }
                m_parent.RebuildJar(t);
            }
        }
        
        public void InventoryAddDecor(ushort id)
        {
            Decor d = new Decor(id, CollectionsSO.LoadedInstance.GetItem(id).name);
            
            if (m_inventoryData.RequestData(-1, ref d))
            {
                d.quantity += 1;
            }
            m_inventoryData.UpdateValue(-1, d);
            m_parent.RebuildJar(d);
        }

        public void InventoryRemoveDecor(ushort id)
        {
            Decor d = new Decor(id, CollectionsSO.LoadedInstance.GetItem(id).name);

            if (m_inventoryData.RequestData(-1, ref d))
            {
                d.quantity -= 1;
                
                if (d.quantity <= 0)
                {
                    m_inventoryData.RemoveComponent(-1, d);
                }
                else
                {
                    m_inventoryData.UpdateValue(-1, d);
                }
                m_parent.RebuildJar(d);
            }
        }
    }
}