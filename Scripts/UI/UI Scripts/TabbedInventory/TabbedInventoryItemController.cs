using GrandmaGreen.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen.UI.Collections
{
    public class TabbedInventoryItemController
    {
        public IInventoryItem m_inventoryItemData;
        
        public Button m_button;

        public TabbedInventoryItemController(Button button)
        {
            m_button = button;
        }

        public void SetButtonCallback(System.Action<TabbedInventoryItemController> buttonClickCallback)
        {
            m_button.clicked += () =>
            {
                buttonClickCallback?.Invoke(this);
            };
        }

        //This function receives the character whose name this list 
        //element is supposed to display. Since the elements list 
        //in a `ListView` are pooled and reused, it's necessary to 
        //have a `Set` function to change which character's data to display.
        public void SetInventoryData(IInventoryItem inventoryItem, Sprite sprite)
        {
            m_button.Q<VisualElement>("item-image").style.backgroundImage = new StyleBackground(sprite);
            m_button.Q<Label>("quantity").text = inventoryItem.GetQuantityToString();
            m_button.Q<Label>("item-name").text = inventoryItem.itemName;
            m_inventoryItemData = inventoryItem;
        }

        public void SetFavorite(Sprite sprite)
        {
            m_button.Q<VisualElement>("list-entry").style.backgroundImage = new StyleBackground(sprite);
        }
    }
}
