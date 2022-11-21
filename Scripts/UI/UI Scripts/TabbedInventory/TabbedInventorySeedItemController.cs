using GrandmaGreen.Collections;
using GrandmaGreen.Garden;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen.UI.Collections
{
    public class TabbedInventorySeedItemController
    {
        private Seed m_seedItem;
        
        private Button m_button;
        
        private System.Action<ushort, Genotype> m_clickCallback;

        public TabbedInventorySeedItemController (Button button, System.Action<ushort, Genotype> clickCallback)
        {
            m_button = button;
            m_clickCallback = clickCallback;

            button.clicked += OnButtonClicked;
        }
        
        public void SetInventoryData(IInventoryItem inventoryItem, Sprite sprite)
        {
            m_button.Q<VisualElement>("item-image").style.backgroundImage = new StyleBackground(sprite);
            m_button.Q<Label>("quantity").text = inventoryItem.GetQuantityToString();
            m_button.Q<Label>("item-name").text = inventoryItem.itemName;
            m_seedItem = (Seed) inventoryItem;
        }

        private void OnButtonClicked()
        {
            m_clickCallback?.Invoke(m_seedItem.itemID, m_seedItem.seedGenotype);
        }
    }
}
