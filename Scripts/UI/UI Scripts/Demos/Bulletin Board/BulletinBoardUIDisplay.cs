using System;
using System.Collections;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using GrandmaGreen.SaveSystem;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

namespace GrandmaGreen
{
    public class BulletinBoardUIDisplay : UIDisplayBase
    {
        public ObjectSaver inventoryObjectSaver;
        
        // Entry template used to spawn info list items.
        public VisualTreeAsset infoListEntryTemplate;
        
        public CollectionsSO collectionsSO;

        public void Start()
        {
            // For daily, announcement, and competition buttons, if clicked open the submission box.
            RegisterButtonCallback("daily", () =>
            {
                m_rootVisualElement.Q<VisualElement>("submissionBoxContainer").style.display = DisplayStyle.Flex;
            });

            RegisterButtonCallback("competition", () =>
            {
                m_rootVisualElement.Q<VisualElement>("submissionBoxContainer").style.display = DisplayStyle.Flex;
            });
            
            RegisterSubmissionBox();
        }

        public override void OpenUI()
        {
            EventManager.instance.HandleEVENT_CLOSE_HUD();
            SetItemSource();
            base.OpenUI();
        }

        public override void CloseUI()
        {
            base.CloseUI();
            EventManager.instance.HandleEVENT_OPEN_HUD();
        }

        public void RegisterSubmissionBox()
        {
            // Register the exit button.
            RegisterButtonCallback("submissionBoxExitButton", () =>
            {
                m_rootVisualElement.Q<VisualElement>("submissionBoxContainer").style.display = DisplayStyle.None;
            });
            
            // Get the list view.
            ListView submissionJar = m_rootVisualElement.Q<ListView>("submissionBoxListView");
            SetItemSource();

            // Instantiate the list view with the appropriate favorite items.
            submissionJar.makeItem = () =>
            {
                // Instantiate the UXML template for the entry
                var newListEntry = infoListEntryTemplate.Instantiate();

                // Return the root of the instantiated visual tree
                return newListEntry;
            };
            
            // Set up bind function for a specific list entry.
            submissionJar.bindItem = (item, index) =>
            {
                // Get this particular plant.
                Plant plant = (Plant) submissionJar.itemsSource[index];
                
                // Get and set the item image.
                Sprite sprite = collectionsSO.GetSprite((PlantId) plant.itemID, plant.genotypes[0], 2);
                item.Q<VisualElement>("plantImage").style.backgroundImage = new StyleBackground(sprite);
                
                // Get the button and set the callback.
                item.Q<Button>("plantFavoriteButton").clicked += () =>
                {
                    // Unselect whatever is currently selected.
                    submissionJar.SetSelection(index);
                };
            };
            
            // On submit, remove plant from inventory, add money (base 100) to inventory, and close the entry box window.
            RegisterButtonCallback("submissionBoxSubmitButton", () =>
            {
                if (submissionJar.selectedIndex >= 0 && submissionJar.selectedIndex < submissionJar.itemsSource.Count)
                {
                    // Close the list view.
                    m_rootVisualElement.Q<VisualElement>("submissionBoxContainer").style.display = DisplayStyle.None;

                    // Add 100 coins.
                    EventManager.instance.HandleEVENT_INVENTORY_ADD_MONEY(200);

                    // Remove plant from inventory.
                    Plant p = (Plant)submissionJar.selectedItem;
                    EventManager.instance.HandleEVENT_INVENTORY_REMOVE_PLANT(p.itemID, p.genotypes[0], true);

                    SetItemSource();
                }
            });
        }
        
        public void SetItemSource()
        {
            // Get the list view.
            ListView submissionJar = m_rootVisualElement.Q<ListView>("submissionBoxListView");

            // Get the correct component store from the object saver.
            ComponentStore<Plant> plantStore = (ComponentStore<Plant>) inventoryObjectSaver.GetComponentStore<Plant>();
            
            // Retrieve the list of FAVORITED plants only.
            List<Plant> favoritedPlants = new List<Plant>();
            
            foreach (Plant p in plantStore.components)
            {
                if (p.isFavorited)
                {
                    favoritedPlants.Add(p);
                }
            }

            submissionJar.itemsSource = favoritedPlants;

            if (submissionJar.visible)
            {
                submissionJar.RefreshItems();
            }
            else
            {
                submissionJar.Rebuild();
            }
        }
    }
}
