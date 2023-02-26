using System.Collections.Generic;
using GrandmaGreen.Collections;
using GrandmaGreen.Mail;
using GrandmaGreen.SaveSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen.UI.BulletinBoard
{
    public class BulletinBoardUIDisplay : UIDisplayBase
    {
        public ObjectSaver inventoryObjectSaver;

        // Entry template used to spawn info list items.
        public VisualTreeAsset infoListEntryTemplate;

        public MailboxModel mailboxModel;
        
        [ShowInInspector]
        public BulletinBoardUIController controller;

        public SpookuleleAudio.ASoundContainer openSFX;
        public SpookuleleAudio.ASoundContainer closeSFX;
        
        // Letter templates, basic in-editor for now.
        // TODO: Let's move this to the mailbox model later?
        public Letter contestLostLetter;
        public Letter contestWonLetter;

        public GameObject player;
        private bool m_isMenuOpen = false;
        private bool m_isInteracting = false;

        public void Start()
        {
            // Instantiate controller.
            controller = new BulletinBoardUIController(this);

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

        private void Update()
        {
            if (m_isInteracting)
            {
                // facing to grandma
                Vector3 playerPos = player.transform.position;

                if ((playerPos - transform.position).sqrMagnitude <= 5.0f)
                {
                    ToggleMenu(true);
                }
                else
                {
                    ToggleMenu(false);
                }
            }
        }

        public override void OpenUI()
        {
            m_isMenuOpen = true;

            SetItemSource();
            openSFX?.Play();
            base.OpenUI();
        }

        public override void CloseUI()
        {
            m_isMenuOpen = false;
            m_isInteracting = false;

            closeSFX?.Play();
            base.CloseUI();
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
                Plant plant = (Plant)submissionJar.itemsSource[index];

                // Get and set the item image.
                Sprite sprite = CollectionsSO.LoadedInstance.GetSprite((PlantId)plant.itemID, plant.plantGenotype, 2);
                item.Q<VisualElement>("plantImage").style.backgroundImage = new StyleBackground(sprite);

                // Get the button and set the callback.
                item.Q<Button>("plantFavoriteButton").clicked += () =>
                {
                    // Unselect whatever is currently selected.
                    submissionJar.SetSelection(index);
                };
            };

            // On submit, remove plant from inventory, check the submission, and close the entry box window.
            RegisterButtonCallback("submissionBoxSubmitButton", () =>
            {
                if (submissionJar.selectedIndex >= 0 && submissionJar.selectedIndex < submissionJar.itemsSource.Count)
                {
                    // Close the list view.
                    m_rootVisualElement.Q<VisualElement>("submissionBoxContainer").style.display = DisplayStyle.None;

                    // Remove plant from inventory.
                    Plant p = (Plant)submissionJar.selectedItem;
                    EventManager.instance.HandleEVENT_INVENTORY_REMOVE_PLANT(p.itemID, p.plantGenotype);

                    SetItemSource();
                    
                    // Check if the submission was good or not.
                    if (controller.EvaluatePlant(p))
                    {
                        EventManager.instance.HandleEVENT_INVENTORY_ADD_MONEY(controller.currentBigContest.rewardMoney);
                        mailboxModel.SendLetterNow(contestWonLetter);
                    }
                    else
                    {
                        mailboxModel.SendLetterNow(contestLostLetter);
                    }
                }
            });
        }

        // TODO: Everything here needs to be more generalized, so that it can work with any contest.
        public void SetItemSource()
        {
            // Get the list view.
            ListView submissionJar = m_rootVisualElement.Q<ListView>("submissionBoxListView");

            // Get the correct component store from the object saver.
            ComponentStore<Plant> plantStore = (ComponentStore<Plant>)inventoryObjectSaver.GetComponentStore<Plant>();

            // Retrieve the list of APPLICABLE plants only.
            List<Plant> applicablePlants = new List<Plant>();

            foreach (Plant p in plantStore.components)
            {
                if (p.itemID == controller.currentBigContest.targetPlant.itemID)
                {
                    applicablePlants.Add(p);
                }
            }

            submissionJar.itemsSource = applicablePlants;

            if (submissionJar.visible)
            {
                submissionJar.RefreshItems();
            }
            else
            {
                submissionJar.Rebuild();
            }
        }

        public void HandleTap()
        {
            if (!m_isMenuOpen)
            {
                EventManager.instance.HandleEVENT_GOLEM_GRANDMA_MOVE_TO(transform.position);
                m_isInteracting = true;
            }
            else
            {
                ToggleMenu(false);
                m_isInteracting = false;
            }
        }

        public void ToggleMenu(bool isOpen)
        {
            if (m_isMenuOpen == isOpen) return;
            m_isMenuOpen = isOpen;

            if (m_isMenuOpen)
            {
                OpenUI();
            }
            else
            {
                CloseUI();
            }
        }

        public void ReleaseShopkeeper()
        {
            if (!m_isInteracting) return;

            m_isInteracting = false;
            ToggleMenu(false);
        }
    }
}
