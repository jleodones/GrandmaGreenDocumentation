using System.Collections;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen.UI.Collections
{
    public class TabbedInventoryItemController
    {

        private VisualElement m_visualElement;

        public TabbedInventoryItemController(VisualElement visualElement)
        {
            m_visualElement = visualElement;
        }

        //This function receives the character whose name this list 
        //element is supposed to display. Since the elements list 
        //in a `ListView` are pooled and reused, it's necessary to 
        //have a `Set` function to change which character's data to display.

        public void SetInventoryData(IInventoryItem inventoryItem)
        {
            m_visualElement.Q<Label>("quantity").text = inventoryItem.GetQuantityToString();
            m_visualElement.Q<Label>("item-name").text = inventoryItem.itemName;
        }
    }
}
