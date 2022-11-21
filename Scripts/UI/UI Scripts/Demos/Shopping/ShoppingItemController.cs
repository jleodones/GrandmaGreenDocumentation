using GrandmaGreen.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.Shopkeeping;
using Id = System.Enum;
using static UnityEditor.Progress;

namespace GrandmaGreen.UI.Shopping
{
    public class ShoppingItemController : MonoBehaviour
    {
        // Shopping item button that this is attached to.
        private VisualElement m_root;
        private Button m_button;
        private System.Action<ShopItem> m_clickCallback;
        private ShopItem thisItem;
        public ShoppingItemController (VisualElement root, Button button, System.Action<ShopItem> clickCallback)
        {
            m_root = root;
            m_button = button;
            m_clickCallback = clickCallback;

            m_button.clicked += OnButtonClicked;
        }
        
        public void SetData(ShopItem shopItem)
        {
            thisItem = shopItem;

            // Set the image on the item.
            m_button.Q<VisualElement>("IconImage").style.backgroundImage = new StyleBackground(thisItem.sprite);
            
            // Set the cost of the item.
            m_root.Q<Label>("Price").text = thisItem.baseCost.ToString();
        }

        private void OnButtonClicked()
        {
            m_clickCallback?.Invoke(thisItem);
        }
    }
}
