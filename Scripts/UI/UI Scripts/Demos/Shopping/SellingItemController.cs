using System;
using System.Collections;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using GrandmaGreen.Garden;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen.UI.Selling
{
    public class SellingItemController
    {
        private SellingUIDisplay m_owner;
        private VisualElement m_rootVisualElement;
        public IInventoryItem item;
        public int currentQuantity;
        private int m_basePrice;

        public SellingItemController(SellingUIDisplay owner)
        {
            m_owner = owner;
        }

        public void SetItemData(VisualElement ve, IInventoryItem newItem, Sprite s, int basePrice, int itemQuantity)
        {
            m_rootVisualElement = ve.Q<VisualElement>("rootVisualElement");
            item = newItem;
            currentQuantity = itemQuantity;
            m_basePrice = basePrice;
            
            // Setting visuals.
            m_rootVisualElement.Q<VisualElement>("item-image").style.backgroundImage = new StyleBackground(s);
            m_rootVisualElement.Q<Label>("quantity").text = currentQuantity.ToString();
            m_rootVisualElement.Q<Label>("price").text = (currentQuantity * m_basePrice).ToString();

            // Set up button callbacks.
            m_rootVisualElement.Q<Button>("minus").RegisterCallback<ClickEvent>(RemoveQuantity);
            m_rootVisualElement.Q<Button>("plus").RegisterCallback<ClickEvent>(AddQuantity);
            m_rootVisualElement.Q<Button>("cancel").RegisterCallback<ClickEvent>(OnCancelClicked);
            
            SetBadgeSize();
        }

        public void SetBadgeSize()
        {
            Genotype genotype;
            if (item.GetType() == new Seed().GetType())
            {
                genotype = ((Seed)item).seedGenotype;
            }
            else
            {
                genotype = ((Plant)item).plantGenotype;
            }

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
            
            m_rootVisualElement.Q<VisualElement>("size-badge").style.backgroundImage = new StyleBackground(sprite);
            m_rootVisualElement.Q<VisualElement>("size-badge").style.display = DisplayStyle.Flex;
        }
        public void OnCancelClicked(ClickEvent ce)
        {
            m_owner.RemoveItem(item);
        }

        private void AddQuantity(ClickEvent ce)
        {
            if (currentQuantity == item.quantity)
            {
                return;
            }
            else
            {
                currentQuantity += 1;
            }

            m_rootVisualElement.Q<Label>("quantity").text = currentQuantity.ToString();
            m_rootVisualElement.Q<Label>("price").text = (currentQuantity * m_basePrice).ToString();

            m_owner.AdjustItemQuantity(item, currentQuantity);
            m_owner.AdjustSalesTotal(true, m_basePrice);
        }

        private void RemoveQuantity(ClickEvent ce)
        {
            if (currentQuantity == 1)
            {
                return;
            }
            else
            {
                currentQuantity -= 1;
            }

            m_rootVisualElement.Q<Label>("quantity").text = currentQuantity.ToString();
            m_rootVisualElement.Q<Label>("price").text = (currentQuantity * m_basePrice).ToString();
            
            m_owner.AdjustItemQuantity(item, currentQuantity);
            m_owner.AdjustSalesTotal(false, m_basePrice);
        }
    }
}
