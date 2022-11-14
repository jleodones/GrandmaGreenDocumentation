using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.SaveSystem;
using GrandmaGreen.Collections;
using GrandmaGreen.Garden;
using GrandmaGreen.Shopkeeping;
using Sirenix.OdinInspector;

namespace GrandmaGreen.UI.Shopping
{
    public class ShoppingUI : MonoBehaviour
    {
        private VisualElement m_rootVisualElement;

        [SerializeField] private VisualTreeAsset m_shoppingItemTemplate;
        
        public List<ShopItem> availableItems;
        
        // Stuff for the shopping amount. This should theoretically get sent to the ShoppingUIController, but will be handled here for now.
        public int m_currentAmount = 1;
        private ShopItem m_currentItem;
        
        void OnEnable()
        {
            m_rootVisualElement = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("rootElement");
            m_rootVisualElement.Q<Button>("exitButton").RegisterCallback<ClickEvent>(RegisterExitButton);
            
            // Setting money.
            m_rootVisualElement.Q<Label>("GoldAmount").text =
                EventManager.instance.HandleEVENT_INVENTORY_GET_MONEY().ToString();
            
            // Manually making the items list.
            
            // Set up the popup window.
            RegisterPopupWindow();

            // Add a call to the shopping UI controller here. For now, the available items list is hardcoded.

            InstantiateJar();
        }
        public void OpenUI()
        {
            EventManager.instance.HandleEVENT_CLOSE_HUD();
            m_rootVisualElement.style.display = DisplayStyle.Flex;
        }

        private void RegisterExitButton(ClickEvent cvt)
        {
            m_rootVisualElement.style.display = DisplayStyle.None;
            EventManager.instance.HandleEVENT_OPEN_HUD();
        }
        
        void InstantiateJar()
        {
            // ScrollView contentJar = m_rootVisualElement.Q<ScrollView>("contentJar");
            
            // For each element in the shopping item list, attach it to the content jar.
            for (int i = 1; i <= 8; i++)
            {
                VisualElement ve = m_rootVisualElement.Q<VisualElement>("content-" + i.ToString());
                var controller = new ShoppingItemController(ve, ve.Q<Button>("IconButton"), OnShoppingItemClicked);
                controller.SetData(availableItems[i - 1]);
                ve.userData = controller;
            }
        }

        void RegisterPopupWindow()
        {
            // Register exit button.
            m_rootVisualElement.Q<Button>("PopUpExitButton").clicked += () =>
            {
                m_rootVisualElement.Q("PopUpScreen").style.display = DisplayStyle.None;
                m_currentAmount = 1;
            };
            
            // Register up and down button clicks.
            m_rootVisualElement.Q<Button>("RemoveButton").clicked += () =>
            {
                // Decrement current amount.
                m_currentAmount -= 1;
                if (m_currentAmount < 1)
                {
                    m_currentAmount = 1;
                }

                m_rootVisualElement.Q<Label>("CurrentAmount").text = m_currentAmount.ToString();
                
                // Update the price text.
                m_rootVisualElement.Q<Label>("CurrentPrice").text = (m_currentItem.baseCost * m_currentAmount).ToString();
            };

            m_rootVisualElement.Q<Button>("AddButton").clicked += () =>
            {
                // Increment current amount.
                m_currentAmount += 1;
                if (m_currentAmount > 10)
                {
                    m_currentAmount = 10;
                }
                m_rootVisualElement.Q<Label>("CurrentAmount").text = m_currentAmount.ToString();

                // Update the price text.
                m_rootVisualElement.Q<Label>("CurrentPrice").text = (m_currentItem.baseCost * m_currentAmount).ToString();
            };
            
            // Register buy button.
            m_rootVisualElement.Q<Button>("BuyButton").clicked += () =>
            {
                int currentMoney = EventManager.instance.HandleEVENT_INVENTORY_GET_MONEY();

                if (currentMoney < (m_currentAmount * m_currentItem.baseCost))
                {
                    return;
                }
                
                // Seed.
                if (m_currentItem.type == ItemType.Seed)
                {
                    for (int i = 0; i < m_currentAmount; i++)
                    {
                        EventManager.instance.HandleEVENT_INVENTORY_ADD_PLANT_OR_SEED(
                            new Seed(m_currentItem.id, m_currentItem.itemName, new List<Genotype>()), new Genotype());
                    }
                }

                // Tool.
                else if (m_currentItem.type == ItemType.Tool)
                {
                    EventManager.instance.HandleEVENT_INVENTORY_ADD_TOOL_OR_DECOR(new Tool(m_currentItem.id, m_currentItem.itemName, 1), m_currentAmount);
                }
                
                Debug.Log("Buying for: " + m_currentAmount + " * " + m_currentItem.baseCost);
                
                EventManager.instance.HandleEVENT_INVENTORY_REMOVE_MONEY(m_currentAmount * m_currentItem.baseCost);
                
                m_rootVisualElement.Q<Label>("GoldAmount").text =
                    EventManager.instance.HandleEVENT_INVENTORY_GET_MONEY().ToString();
            };
        }
        
        void OnShoppingItemClicked(ShopItem item)
        {
            m_currentItem = item;
            
            // Set the name.
            m_rootVisualElement.Q<Label>("ItemName").text = item.itemName;
            
            // Set the sprite image.
            m_rootVisualElement.Q("Item").style.backgroundImage = new StyleBackground(item.sprite);
            
            // Set the starting price.
            m_rootVisualElement.Q<Label>("CurrentPrice").text = item.baseCost.ToString();
            
            // Open up the popup window.
            m_rootVisualElement.Q("PopUpScreen").style.display = DisplayStyle.Flex;
        }
    }
}
