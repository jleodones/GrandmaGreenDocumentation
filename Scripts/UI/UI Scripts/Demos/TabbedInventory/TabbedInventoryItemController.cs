using System.Collections;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.Garden;

namespace GrandmaGreen.UI.Collections
{
    public class TabbedInventoryItemController
    {
        private Button m_button;
        System.Action<int> m_clickCallback;
        int itemID;
        public TabbedInventoryItemController(Button button, System.Action<int> clickCallback)
        {
            m_button = button;
            m_clickCallback = clickCallback;

            button.clicked += OnButtonClicked;
        }

        //This function receives the character whose name this list 
        //element is supposed to display. Since the elements list 
        //in a `ListView` are pooled and reused, it's necessary to 
        //have a `Set` function to change which character's data to display.

        public void SetInventoryData(IInventoryItem inventoryItem)
        {
            m_button.Q<Label>("quantity").text = inventoryItem.GetQuantityToString();
            m_button.Q<Label>("item-name").text = inventoryItem.itemName;
            itemID = inventoryItem.itemID;
        }

        void CheckOpenInventory(ToolData selectedTool)
        {

        }

        void OnButtonClicked()
        {
            m_clickCallback?.Invoke(itemID);
        }


    }
}
