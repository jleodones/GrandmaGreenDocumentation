using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GrandmaGreen.Collections;
using GrandmaGreen.Shopkeeping;
using GrandmaGreen.UI.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen
{
    public class SellingUIDisplay : UIDisplayBase
    {
        public TabbedInventory inventoryUI;
        private List<Tuple<IInventoryItem, int>> m_items;
        private ListView m_sellingBox;
        public VisualTreeAsset sellingItemTemplate;

        private GardeningShopUIController m_controller;
        private int m_currentSalesTotal = 0;
        void Start()
        {
            m_controller = new GardeningShopUIController();
            m_sellingBox = m_rootVisualElement.Q<ListView>("content-jar");
            m_sellingBox.Q<ScrollView>().verticalScrollerVisibility = ScrollerVisibility.Hidden;
            
            // Callbacks.
            inventoryUI.RegisterButtonCallback("exitButton", CloseUI);
            RegisterButtonCallback("sellButton", SellItems);
            
            // Jar.
            m_items = new List<Tuple<IInventoryItem, int>>();
            SetupJar();
        }
        
        public override void OpenUI()
        {
            // Open inventory UI along with itself.
            inventoryUI.currentMode = InventoryMode.Selling;
            inventoryUI.OpenUI();
            m_items.Clear();
            
            // Since it should be empty, show empty box UI.
            m_rootVisualElement.Q<VisualElement>("empty").style.display = DisplayStyle.Flex;
            
            // Set sales total.
            m_rootVisualElement.Q<Label>("sale-total").text = m_currentSalesTotal.ToString();
            
            // Set money.
            m_rootVisualElement.Q<Label>("coins-amount").text = EventManager.instance.HandleEVENT_INVENTORY_GET_MONEY().ToString();

            // Then itself.
            base.OpenUI();
        }

        public override void CloseUI()
        {
            // Clear selling box data.
            inventoryUI.currentMode = InventoryMode.Customization;
            inventoryUI.CloseUI();
            m_items.Clear();

            // Then close.
            base.CloseUI();
        }

        public bool IsInBounds(Vector3 itemPosition)
        {
            return m_rootVisualElement.Q<VisualElement>("selling-bg").worldBound.Contains(itemPosition);
        }

        public void AddItem(IInventoryItem item)
        {
            if (m_controller.IsSellable(item))
            {
                if (m_items.Count == 0)
                {
                    m_rootVisualElement.Q<VisualElement>("empty").style.display = DisplayStyle.None;
                }

                var n = new Tuple<IInventoryItem, int>(item, item.quantity);
                m_items.Add(n);
                AdjustSalesTotal(true, m_controller.GetSellingPriceById(n.Item1) * n.Item1.quantity);
                m_sellingBox.RefreshItems();
            }
        }

        public void RemoveItem(IInventoryItem item)
        {
            var n = m_items.First( i => i.Item1 == item);
            AdjustSalesTotal(false, m_controller.GetSellingPriceById(n.Item1) * n.Item2);
            
            m_items.Remove(n);
            m_sellingBox.RefreshItems();
            
            // If empty, bring back empty box screen.
            if (m_items.Count == 0)
            {
                m_rootVisualElement.Q<VisualElement>("empty").style.display = DisplayStyle.Flex;
            }
        }

        public void SellItems()
        {
            // Remove all items in list from inventory.
            var list = m_sellingBox.Query<VisualElement>(className: "unity-list-view__item").ToList();
            
            foreach (VisualElement ve in list)
            {
                SellingItemController ic = ve.userData as SellingItemController;
                for (int i = 0; i < ic.currentQuantity; i++)
                {
                    if (ic.item.GetType() == new Seed().GetType())
                    {
                        EventManager.instance.HandleEVENT_INVENTORY_REMOVE_SEED(ic.item.itemID, ((Seed)ic.item).seedGenotype);
                    }
                    else
                    {
                        EventManager.instance.HandleEVENT_INVENTORY_REMOVE_PLANT(ic.item.itemID, ((Plant)ic.item).plantGenotype);
                    }
                }
            }

            // Add money to inventory.
            EventManager.instance.HandleEVENT_INVENTORY_ADD_MONEY(m_currentSalesTotal);
            
            // Set sales total.
            m_currentSalesTotal = 0;
            m_rootVisualElement.Q<Label>("sale-total").text = m_currentSalesTotal.ToString();
            
            // Clear all items.
            m_items.Clear();
            m_sellingBox.RefreshItems();
            inventoryUI.RebuildAllJars();
            
            // Adjust money.
            m_rootVisualElement.Q<Label>("coins-amount").text = EventManager.instance.HandleEVENT_INVENTORY_GET_MONEY().ToString();
        }

        public void AdjustItemQuantity(IInventoryItem item, int quantity)
        {
            var n = m_items.FindIndex( i => i.Item1 == item);
            m_items[n] = new Tuple<IInventoryItem, int>(item, quantity);
        }

        public void AdjustSalesTotal(bool isBeingAdded, int price)
        {
            if (isBeingAdded) m_currentSalesTotal += price;
            else m_currentSalesTotal -= price;
            
            // Set sales total.
            m_rootVisualElement.Q<Label>("sale-total").text = m_currentSalesTotal.ToString();
        }
        
        // Sets up the item binding for the inventory UI.
        private void SetupJar()
        {
            // Set up item source.
            m_sellingBox.itemsSource = m_items;

            // Set up a make item function for a list entry.
            m_sellingBox.makeItem = () =>
            {
                // Instantiate the UXML template for the entry
                var newListEntry = sellingItemTemplate.Instantiate();
                
                var newListEntryLogic =
                    new SellingItemController(this);

                newListEntry.userData = newListEntryLogic;
                
                // Return the root of the instantiated visual tree
                return newListEntry;
            };
            
            // Set up bind function for a specific list entry.
            m_sellingBox.bindItem = (item, index) =>
            {
                Sprite s;
                if (m_items[index].GetType() == new Seed().GetType())
                {
                    s = CollectionsSO.LoadedInstance.GetSprite((PlantId) m_items[index].Item1.itemID, ((Seed) m_items[index].Item1).seedGenotype);
                }
                else
                {
                    s = CollectionsSO.LoadedInstance.GetInventorySprite((PlantId) m_items[index].Item1.itemID, ((Plant) m_items[index].Item1).plantGenotype);
                }
                int basePrice = m_controller.GetSellingPriceById(m_items[index].Item1);
                
                (item.userData as SellingItemController).SetItemData(item, m_items[index].Item1, s, basePrice, m_items[index].Item2);
            };
        }
    }
}
