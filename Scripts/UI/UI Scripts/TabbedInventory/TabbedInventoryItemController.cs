using GrandmaGreen.Collections;
using UnityEngine.UIElements;

namespace GrandmaGreen.UI.Collections
{
    public class TabbedInventoryItemController
    {
        public IInventoryItem m_inventoryItemData;
        
        public Button m_button;
        
        public event System.Action<TabbedInventoryItemController> m_createCallback;

        public TabbedInventoryItemController(Button button, System.Action<TabbedInventoryItemController> createCallback)
        {
            m_button = button;
            m_createCallback = createCallback;
            m_createCallback?.Invoke(this);
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
    }
}
