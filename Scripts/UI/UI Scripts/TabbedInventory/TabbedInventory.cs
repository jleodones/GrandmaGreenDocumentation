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

        // Inventory scriptable object with all inventory data.
        public ObjectSaver inventoryData;
        
        private TabbedInventoryController m_controller;

        public ASoundContainer[] soundContainers;

        private Length inventoryWidth = (Length)(.45 * Screen.width);

        void OnEnable()
        {
            // Gets the root of the tabbed inventory, which holds all the tabs in it.
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            // Sets up the controller for the whole inventory. The controller instantiates the inventory on its own upon creation.
            m_controller = new(root, playerToolData, inventoryData, listEntryTemplate, soundContainers);

            // Hides the inventory without animation
            SetInventoryPosition();

            // Register player events.
            m_controller.RegisterTabCallbacks();
            m_controller.RegisterExitCallback();
            
            // Subscribe to inventory related events.
            EventManager.instance.EVENT_INVENTORY_OPEN += OpenInventory;
            EventManager.instance.EVENT_INVENTORY_ADD_TOOL_OR_DECOR += InventoryAddToolOrDecor;
            EventManager.instance.EVENT_INVENTORY_REMOVE_TOOL_OR_DECOR += InventoryRemoveToolOrDecor;
            
            EventManager.instance.EVENT_INVENTORY_ADD_PLANT_OR_SEED += InventoryAddPlantOrSeed;
            EventManager.instance.EVENT_INVENTORY_REMOVE_PLANT_OR_SEED += InventoryRemovePlantOrSeed;
        }

        public void SetInventoryPosition()
        {
            m_controller.SetInventoryPosition();
        }

        public void OpenInventory()
        {
            m_controller.OpenInventory();
        }

        private void InventoryAddToolOrDecor(IInventoryItem item, int num)
        {
            switch (item.itemType)
            {
                case ItemType.Tool:
                    // Update the inventory SO.
                    Tool t = (Tool)item;

                    if (inventoryData.RequestData<GrandmaGreen.Collections.Tool>(-1, ref t))
                    {
                        t.quantity += num;
                    }

                    inventoryData.UpdateValue<GrandmaGreen.Collections.Tool>(-1, t);

                    // Update the UI system.
                    m_controller.RebuildJar(t);
                    break;
                case ItemType.Decor:
                    // Update the inventory SO.
                    Decor d = (Decor)item;

                    if (inventoryData.RequestData<Decor>(-1, ref d))
                    {
                        d.quantity += num;
                    }

                    inventoryData.UpdateValue<Decor>(-1, d);

                    // Update the UI system.
                    m_controller.RebuildJar(d);
                    break;
            }
        }
        
        private void InventoryRemoveToolOrDecor(IInventoryItem item, int num)
        {
            switch (item.itemType)
            {
                case ItemType.Tool:
                    // Update the inventory SO.
                    GrandmaGreen.Collections.Tool t = (GrandmaGreen.Collections.Tool)item;

                    // If it exists, remove the appropriate amount.
                    if (inventoryData.RequestData<GrandmaGreen.Collections.Tool>(-1, ref t))
                    {
                        t.quantity -= num;
                        if (t.quantity <= 0)
                        {
                            inventoryData.RemoveComponent<GrandmaGreen.Collections.Tool>(-1, t);
                        }
                        else
                        {
                            inventoryData.UpdateValue<GrandmaGreen.Collections.Tool>(-1, t);
                        }

                        // Then update the UI system.
                        m_controller.RebuildJar(t);
                    }

                    break;
                case ItemType.Decor:
                    // Update the inventory SO.
                    Decor d = (Decor)item;

                    // If it exists, remove the appropriate amount.
                    if (inventoryData.RequestData<Decor>(-1, ref d))
                    {
                        d.quantity -= num;
                        if (d.quantity <= 0)
                        {
                            inventoryData.RemoveComponent<Decor>(-1, d);
                        }
                        else
                        {
                            inventoryData.UpdateValue<Decor>(-1, d);
                        }

                        // Then update the UI system.
                        m_controller.RebuildJar(d);
                    }
                    break;
            }
        }

        private void InventoryAddPlantOrSeed(IInventoryItem item, Genotype genotype)
        {
            switch (item.itemType)
            {
                case ItemType.Seed :
                    // Update the inventory SO.
                    Seed s = (Seed) item;
                    
                    if (inventoryData.RequestData<Seed>(-1, ref s))
                    {
                        s.genotypes.Add(genotype);
                    }
                    inventoryData.UpdateValue<Seed>(-1, s);
                    
                    // Update the UI system.
                    m_controller.RebuildJar(s);
                    break;
                case ItemType.Plant :
                    // Update the inventory SO.
                    Plant p = (Plant)item;
            
                    if (inventoryData.RequestData<Plant>(-1, ref p))
                    {
                        p.genotypes.Add(genotype);
                    }
                    inventoryData.UpdateValue<Plant>(-1, p);
                    
                    // Update the UI system.
                    m_controller.RebuildJar(p);
                    break;
            }
        }
        
        private void InventoryRemovePlantOrSeed(IInventoryItem item, Genotype genotype)
        {
            switch (item.itemType)
            {
                case ItemType.Seed :
                    // Update the inventory SO.
                    Seed s = (Seed)item;

                    // If it exists, remove the appropriate amount.
                    if (inventoryData.RequestData<Seed>(-1, ref s))
                    {
                        s.genotypes.Remove(genotype);
                        
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
                    break;
                case ItemType.Plant :
                    // Update the inventory SO.
                    Plant p = (Plant)item;

                    // If it exists, remove the appropriate amount.
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
                        // Then update the UI system.
                        m_controller.RebuildJar(p);
                    }
                    break;
            }
        }
    }
}