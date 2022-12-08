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
    public class ShoppingUI : UIDisplayBase
    {
        [SerializeField] private CollectionsSO collections;
        
        public List<ShopItem> availableItems;

        private GardeningShopUIController m_controller;
        
        // Stuff for the shopping amount. This should theoretically get sent to the ShoppingUIController, but will be handled here for now.
        public int m_currentAmount = 1;
        private ShopItem m_currentItem;
        
        void Start()
        {
            // Setting money.
            m_rootVisualElement.Q<Label>("GoldAmount").text =
                EventManager.instance.HandleEVENT_INVENTORY_GET_MONEY().ToString();
            
            // Controller set up.
            m_controller = new GardeningShopUIController(collections);
            availableItems = m_controller.GetGardenList();

            // Set up the popup window.
            RegisterPopupWindow();
            
            InstantiateJar();
        }

        public override void OpenUI()
        {
            EventManager.instance.HandleEVENT_CLOSE_HUD();
            base.OpenUI();
        }

        public override void CloseUI()
        {
            EventManager.instance.HandleEVENT_OPEN_HUD();
            base.CloseUI();
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
                Seed s = (Seed) m_currentItem.myItem;
                
                if (currentMoney < m_currentAmount * m_currentItem.baseCost)
                {
                    return;
                }
                
                for (int i = 0; i < m_currentAmount; i++)
                {
                    EventManager.instance.HandleEVENT_INVENTORY_ADD_SEED(s.itemID, s.seedGenotype);
                }
                
                EventManager.instance.HandleEVENT_INVENTORY_REMOVE_MONEY(m_currentAmount * m_currentItem.baseCost);
                
                m_rootVisualElement.Q<Label>("GoldAmount").text =
                    EventManager.instance.HandleEVENT_INVENTORY_GET_MONEY().ToString();
                
                // Close the window.
                m_rootVisualElement.Q("PopUpScreen").style.display = DisplayStyle.None;
                m_currentAmount = 1;
            };
        }
        
        void OnShoppingItemClicked(ShopItem item)
        {
            m_currentItem = item;
            
            // Set the name.
            m_rootVisualElement.Q<Label>("ItemName").text = m_currentItem.name;
            
            // Set the sprite image.
            m_rootVisualElement.Q("Item").style.backgroundImage = new StyleBackground(m_currentItem.sprite);
            
            // Set the current amount.
            m_rootVisualElement.Q<Label>("CurrentAmount").text = m_currentAmount.ToString();

            // Set the starting price.
            m_rootVisualElement.Q<Label>("CurrentPrice").text = m_currentItem.baseCost.ToString();
            
            // Open up the popup window.
            m_rootVisualElement.Q("PopUpScreen").style.display = DisplayStyle.Flex;
        }
    }
}
