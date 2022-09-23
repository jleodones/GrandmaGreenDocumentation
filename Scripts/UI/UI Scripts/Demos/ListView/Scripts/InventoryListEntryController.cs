using UnityEngine.UIElements;
using UnityEngine;

public class InventoryListEntryController
{
    VisualElement m_IconElement;

    //This function retrieves a reference to the 
    //character name label inside the UI element.

    public void SetVisualElement(VisualElement visualElement)
    {
        m_IconElement = visualElement.Q<VisualElement>("inventory-image");
    }

    //This function receives the character whose name this list 
    //element is supposed to display. Since the elements list 
    //in a `ListView` are pooled and reused, it's necessary to 
    //have a `Set` function to change which character's data to display.

    public void SetInventoryData(InventoryData inventoryData)
    {
        m_IconElement.style.backgroundImage = new StyleBackground(inventoryData.m_ItemImage);
    }
}