// This script attaches the tabbed menu logic to the game.

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.SaveSystem;
using GrandmaGreen.Collections;

//Inherits from class `MonoBehaviour`. This makes it attachable to a game object as a component.
namespace GrandmaGreen.UI.Collections
{
    public class TabbedInventory : MonoBehaviour
    {
        public ToolTest toolScript;

        // Template for list items.
        public VisualTreeAsset listEntryTemplate;

        // Inventory scriptable object with all inventory data.
        public ObjectSaver inventoryData;
        
        private TabbedInventoryController m_controller;

        void OnEnable()
        {
            // Gets the root of the tabbed inventory, which holds all the tabs in it.
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            // Sets up the controller for the whole inventory. The controller instantiates the inventory on its own upon creation.
            m_controller = new(root, toolScript, inventoryData, listEntryTemplate);

            // Register player events.
            m_controller.RegisterTabCallbacks();
            m_controller.RegisterExitCallback();
            
            // Subscribe to inventory related events.
            EventManager.instance.EVENT_INVENTORY_ADD += InventoryAdd;
            EventManager.instance.EVENT_INVENTORY_REMOVE += InventoryRemove;
        }

        public void OpenInventory()
        {
            m_controller.OpenInventory();
        }
        
        public void InventoryAdd(IInventoryItem item, int num)
        {
            switch (item.itemType)
            {
                case ItemType.Tool :
                    // Update the inventory SO.
                    GrandmaGreen.Collections.Tool t = (GrandmaGreen.Collections.Tool)item;
            
                    if (inventoryData.RequestData<GrandmaGreen.Collections.Tool>(-1, ref t))
                    {
                        t.quantity += num;
                    }
                    inventoryData.UpdateValue<GrandmaGreen.Collections.Tool>(-1, t);
                    
                    // Update the UI system.
                    m_controller.RebuildJar(t);
                    break;
                case ItemType.Decor :
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
                case ItemType.Seed :
                    // Update the inventory SO.
                    Seed s = (Seed)item;
            
                    if (inventoryData.RequestData<Seed>(-1, ref s))
                    {
                        s.quantity += num;
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
                        p.quantity += num;
                    }
                    inventoryData.UpdateValue<Plant>(-1, p);
                    
                    // Update the UI system.
                    m_controller.RebuildJar(p);
                    break;
            }
        }

        public void InventoryRemove(IInventoryItem item, int num)
        {
            switch (item.itemType)
            {
                case ItemType.Tool :
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
                case ItemType.Decor :
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
                case ItemType.Seed :
                    // Update the inventory SO.
                    Seed s = (Seed)item;

                    // If it exists, remove the appropriate amount.
                    if (inventoryData.RequestData<Seed>(-1, ref s))
                    {
                        s.quantity -= num;
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
                        p.quantity -= num;
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