using System;
using GrandmaGreen.Collections;
using GrandmaGreen.Garden;
using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.Shopkeeping;
using Id = System.Enum;

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

        public void SetSizeBadge()
        {
            Genotype genotype = ((Seed)thisItem.myItem).seedGenotype;
            
            Sprite sprite;
            // Set size badge.
            String str = "UI_Inventory_Badge_";
            switch (genotype.size)
            {
                case Genotype.Size.VerySmall:
                    str += "XS";
                    break;
                case Genotype.Size.Small:
                    str += "S";
                    break;
                case Genotype.Size.Medium:
                    str += "M";
                    break;
                case Genotype.Size.Big:
                    str += "L";
                    break;
                case Genotype.Size.VeryBig:
                    str += "XL";
                    break;
            }

            sprite = Resources.Load<Sprite>(str);
            
            m_button.Q<VisualElement>("SizeIcon").style.backgroundImage = new StyleBackground(sprite);
            m_button.Q<VisualElement>("SizeIcon").style.display = DisplayStyle.Flex;
        }

        private void OnButtonClicked()
        {
            m_clickCallback?.Invoke(thisItem);
        }
    }
}
