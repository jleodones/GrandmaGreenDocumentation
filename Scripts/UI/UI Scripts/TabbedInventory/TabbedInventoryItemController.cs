using GrandmaGreen.Collections;
using UnityEngine.UIElements;

namespace GrandmaGreen.UI.Collections
{
    public class TabbedInventoryItemController
    {
        private IInventoryItem m_inventoryItemData;
        
        private Button m_button;
        
        private System.Action<ushort> m_clickCallback;

        public TabbedInventoryItemController(Button button, System.Action<ushort> clickCallback)
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

            m_inventoryItemData = inventoryItem;
        }

        private void OnButtonClicked()
        {
            m_clickCallback?.Invoke(m_inventoryItemData.itemID);
        }
    }
}
