// This script attaches the tabbed menu logic to the game.

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.SaveSystem;
using GrandmaGreen.Collections;
using GrandmaGreen.Garden;
using SpookuleleAudio;

//Inherits from class `MonoBehaviour`. This makes it attachable to a game object as a component.
namespace GrandmaGreen.UI.Collections
{
    public class TabbedInventory : MonoBehaviour
    {

        public PlayerToolData playerToolData;

        // Template for list items.
        public VisualTreeAsset listEntryTemplate;
        
        // Collections SO.
        public CollectionsSO collectionsSO;

        // Inventory scriptable object with all inventory data.
        public ObjectSaver inventoryData;
        
        private TabbedInventoryController m_controller;

        public ASoundContainer[] soundContainers;

        private Length inventoryWidth = (Length)(.45 * Screen.width);

        void Awake()
        {
            // Gets the root of the tabbed inventory, which holds all the tabs in it.
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            // Sets up the controller for the whole inventory. The controller instantiates the inventory on its own upon creation.
            m_controller = new(root, playerToolData, inventoryData, listEntryTemplate, soundContainers, collectionsSO);

            // Hides the inventory without animation
            SetInventoryPosition();

            // Register player events.
            m_controller.RegisterTabCallbacks();
            m_controller.RegisterExitCallback();
            
            // Subscribe to inventory related events.
            EventManager.instance.EVENT_INVENTORY_OPEN += OpenInventory;

            EventManager.instance.EVENT_INVENTORY_ADD_PLANT += InventoryAddPlant;
            EventManager.instance.EVENT_INVENTORY_REMOVE_PLANT += InventoryRemovePlant;

            EventManager.instance.EVENT_INVENTORY_ADD_SEED += InventoryAddSeed;
            EventManager.instance.EVENT_INVENTORY_REMOVE_SEED += InventoryRemoveSeed;

            EventManager.instance.EVENT_INVENTORY_ADD_TOOL += InventoryAddTool;
            EventManager.instance.EVENT_INVENTORY_REMOVE_TOOL += InventoryRemoveTool;

            EventManager.instance.EVENT_INVENTORY_ADD_DECOR += InventoryAddDecor;
            EventManager.instance.EVENT_INVENTORY_REMOVE_DECOR += InventoryRemoveDecor;

            // Money.
            EventManager.instance.EVENT_INVENTORY_ADD_MONEY += (int money) =>
            {
                // Adds to the singular money component store (int).
                int currentMoney = money;
                if (inventoryData.RequestData<int>(0, ref currentMoney))
                {
                    currentMoney += money;
                    inventoryData.UpdateValue<int>(0, currentMoney);
                }
            };

            EventManager.instance.EVENT_INVENTORY_REMOVE_MONEY += InventoryRemoveMoney;

            EventManager.instance.EVENT_INVENTORY_GET_MONEY += () =>
            {
                int currentMoney = 0;
                inventoryData.RequestData<int>(0, ref currentMoney);
                return currentMoney;
            };
        }

        public void SetInventoryPosition()
        {
            m_controller.SetInventoryPosition();
        }

        public void OpenInventory()
        {
            m_controller.OpenInventory();
        }

        private void InventoryRemoveMoney(int money)
        {
            int currentMoney = money;
            if (inventoryData.RequestData<int>(0, ref currentMoney))
            {
                currentMoney -= money;
                inventoryData.UpdateValue<int>(0, currentMoney);
            }
        }

        private void InventoryAddSeed(ushort id, Genotype genotype)
        {
            Seed s = new Seed(id, collectionsSO.GetItem(id).name, genotype);

            if (inventoryData.RequestData(-1, ref s))
            {
                s.quantity += 1;
            }
            inventoryData.UpdateValue(-1, s);
                    
            // Update the UI system.
            m_controller.RebuildJar(s);
        }

        private void InventoryRemoveSeed(ushort id, Genotype genotype)
        {
            Seed s = new Seed(id, collectionsSO.GetItem(id).name, genotype);

            if (inventoryData.RequestData(-1, ref s))
            {
                s.quantity -= 1;

                if (s.quantity <= 0)
                {
                    inventoryData.RemoveComponent<Seed>(-1, s);
                }
                else
                {
                    inventoryData.UpdateValue<Seed>(-1, s);
                }
                        
                // Then update the UI system.
                m_controller.RebuildJar(s);
            }
        }

        private void InventoryAddPlant(ushort id, Genotype genotype)
        {
            Plant p = new Plant(id, collectionsSO.GetItem(id).name, genotype);

            if (inventoryData.RequestData(-1, ref p))
            {
                p.genotypes.Add(genotype);
            }
            else
            {
                inventoryData.UpdateValue(-1, p);
            }
            m_controller.RebuildJar(p);
        }
        
        private void InventoryRemovePlant(ushort id, Genotype genotype)
        {
            Plant p = new Plant(id, collectionsSO.GetItem(id).name, genotype);
            
            if (inventoryData.RequestData<Plant>(-1, ref p))
            {
                p.genotypes.Remove(genotype);
                        
                if (p.quantity <= 0)
                {
                    inventoryData.RemoveComponent<Plant>(-1, p);
                }
                else
                {
                    inventoryData.UpdateValue<Plant>(-1, p);
                }
                m_controller.RebuildJar(p);
            }
        }

        private void InventoryAddTool(ushort id)
        {
            Tool t = new Tool(id, collectionsSO.GetItem(id).name);
            
            if (inventoryData.RequestData(-1, ref t))
            {
                t.quantity += 1;
            }
            inventoryData.UpdateValue(-1, t);
            m_controller.RebuildJar(t);
        }

        private void InventoryRemoveTool(ushort id)
        {
            Tool t = new Tool(id, collectionsSO.GetItem(id).name);

            if (inventoryData.RequestData(-1, ref t))
            {
                t.quantity -= 1;
                
                if (t.quantity <= 0)
                {
                    inventoryData.RemoveComponent(-1, t);
                }
                else
                {
                    inventoryData.UpdateValue(-1, t);
                }
                m_controller.RebuildJar(t);
            }
        }
        
        private void InventoryAddDecor(ushort id)
        {
            Decor d = new Decor(id, collectionsSO.GetItem(id).name);
            
            if (inventoryData.RequestData(-1, ref d))
            {
                d.quantity += 1;
            }
            inventoryData.UpdateValue(-1, d);
            m_controller.RebuildJar(d);
        }

        private void InventoryRemoveDecor(ushort id)
        {
            Decor d = new Decor(id, collectionsSO.GetItem(id).name);

            if (inventoryData.RequestData(-1, ref d))
            {
                d.quantity -= 1;
                
                if (d.quantity <= 0)
                {
                    inventoryData.RemoveComponent(-1, d);
                }
                else
                {
                    inventoryData.UpdateValue(-1, d);
                }
                m_controller.RebuildJar(d);
            }
        }
    }
}