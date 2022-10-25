// This script defines the tab selection logic.
using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

namespace GrandmaGreen.UI.HUD
{
    public class HUDController
    {
        /*
        To-Do:
        - exit buttons: hide the current element, 
            show HUD element
        - buttons -> open element with same name
        */

        /* Define member variables*/

        private VisualElement m_root;

        public HUDController(VisualElement root)
        {
            m_root = root;
        }

        public void OpenHUD()
        {
            m_root.style.display = DisplayStyle.Flex;
        }
        public void OpenHUDAnimated()
        {
            m_root.style.display = DisplayStyle.Flex;
        }
         public void CloseHUD()
        {
            m_root.style.display = DisplayStyle.None;
        }

        public void RegisterButtonCallbacks()
        {
            // For now, this just registers the inventory callback.
            // TODO: Update this for every functional UI.
            Button inventoryButton = m_root.Q<Button>("inventory-button");
            inventoryButton.RegisterCallback<ClickEvent>(InventoryOnClick);
        }

        public void InventoryOnClick(ClickEvent evt)
        {
            // Disables self.
            m_root.style.display = DisplayStyle.None;
            
            // Enables inventory.
            HUD.instance.inventory.OpenInventory();
        }
    }
}