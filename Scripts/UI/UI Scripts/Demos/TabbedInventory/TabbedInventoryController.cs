// This script defines the tab selection logic.
using UnityEngine;
using System;
using System.Collections.Generic;
using GrandmaGreen;
using UnityEngine.UIElements;
using GrandmaGreen.SaveSystem;
using GrandmaGreen.Collections;
using Sirenix.OdinInspector;
using GrandmaGreen.UI.HUD;
using GrandmaGreen.Garden;
using SpookuleleAudio;

namespace GrandmaGreen.UI.Collections
{
    public class TabbedInventoryController
    {
        /* Define member variables*/
        private const string tabClassName = "tab-button";
        private const string currentlySelectedTabClassName = "active-tab";
        private const string unselectedContentClassName = "inactive-content";
        private const string exitButton = "exit-button";
        private const string inventoryElement = "inventory-element";
        private const string contentJar = "content-jar";

        // Tab and tab content have the same prefix but different suffix
        // Define the suffix of the tab name
        private const string tabNameSuffix = "-tab";

        // Define the suffix of the tab content name
        private const string contentNameSuffix = "-content";

        // Tools script.
        public PlayerToolData playerToolData;

        // The root of the inventory UI.
        private readonly VisualElement root;

        // Entry template used to spawn inventory items.
        private VisualTreeAsset listEntryTemplate;

        // Holds all content jars.
        private List<ListView> m_contentJars;

        // Inventory data.
        private ObjectSaver m_inventoryData;

        private ASoundContainer[] m_soundContainers;


        public event System.Action<int> onItemEntryClicked;
        /// <summary>
        /// The TabbedInventoryController is attached to the TabbedInventory UI. It registers and controlls tab switching, as well as loading old and new items into the inventory.
        /// </summary>
        /// <param name="_root">The root of the TabbedInventory UI, which holds the different inventory panels.</param>
        /// <param name="_toolTestScript">Connects the tools tabs to the inventory UI.</param>
        /// <param name="inventoryData"></param>
        /// <param name="_listEntryTemplate"></param>
        public TabbedInventoryController(VisualElement _root, PlayerToolData _playerToolData,
            ObjectSaver inventoryData, VisualTreeAsset _listEntryTemplate, ASoundContainer[] soundContainers)
        {
            // Set member variables.
            root = _root;
            playerToolData = _playerToolData;
            listEntryTemplate = _listEntryTemplate;
            m_inventoryData = inventoryData;
            m_soundContainers = soundContainers;

            // Instantiate the inventory items.
            // First, get all content jars.
            m_contentJars = GetAllContentJars().ToList();

            // Manually instantiate each jar.
            InstantiateJar<GrandmaGreen.Collections.Tool>(m_contentJars.Find(jar => jar.name == "tools" + contentNameSuffix));
            InstantiateJar<Seed>(m_contentJars.Find(jar => jar.name == "seeds" + contentNameSuffix));
            InstantiateJar<Decor>(m_contentJars.Find(jar => jar.name == "decor" + contentNameSuffix));
            InstantiateJar<Plant>(m_contentJars.Find(jar => jar.name == "plants" + contentNameSuffix));

            _playerToolData.onToolSelected += CheckOpenInventory;
        }

        public void OpenInventory()
        {


            // transition-property: scale, rotate, display;
    // transition-duration: 0.5s, 0.5s, 2s;
            root.Q(inventoryElement).style.display = DisplayStyle.Flex;
            
            // Play open inventory SFX.
            m_soundContainers[0].Play();
        }

        public void RegisterTabCallbacks()
        {
            UQueryBuilder<Button> tabs = GetAllTabs();
            tabs.ForEach((Button tab) => { tab.RegisterCallback<ClickEvent>(TabOnClick); });
        }

        // Register the hide function to the event: when exit button is clicked
        public void RegisterExitCallback()
        {
            root.Q<Button>(exitButton).RegisterCallback<ClickEvent>(CloseInventory);
        }

        // This hides the entire inventory panel when the exit button is clicked
        private void CloseInventory(ClickEvent evt)
        {
            // Set the display to none.
            root.Q(inventoryElement).style.display = DisplayStyle.None;

            // Play the inventory close SFX.
            m_soundContainers[1].Play();

            // Open the HUD.
            HUD.HUD.instance.OpenHUD();
        }

        /* Method for the tab on-click event: 
    
           - If it is not selected, find other tabs that are selected, unselect them 
           - Then select the tab that was clicked on
        */
        private void TabOnClick(ClickEvent evt)
        {
            Button clickedTab = evt.currentTarget as Button;
            if (!TabIsCurrentlySelected(clickedTab))
            {
                GetAllTabs().Where(
                    (tab) => tab != clickedTab && TabIsCurrentlySelected(tab)
                ).ForEach(UnselectTab);
                SelectTab(clickedTab);
            }
        }

        //Method that returns a Boolean indicating whether a tab is currently selected
        private static bool TabIsCurrentlySelected(Button tab)
        {
            return tab.ClassListContains(currentlySelectedTabClassName);
        }

        private UQueryBuilder<Button> GetAllTabs()
        {
            return root.Query<Button>(className: tabClassName);
        }

        private UQueryBuilder<ListView> GetAllContentJars()
        {
            return root.Query<ListView>(className: contentJar);
        }

        /* Method for the selected tab: 
           -  Takes a tab as a parameter and adds the currentlySelectedTab class
           -  Then finds the tab content and removes the unselectedContent class */
        private void SelectTab(Button tab)
        {
            tab.AddToClassList(currentlySelectedTabClassName);
            ListView content = FindContent(tab);
            content.RemoveFromClassList(unselectedContentClassName);
            // Sending tab name to Ashley

            //TODO: SEND SELECTED TOOL
            //toolTestScript.SetTools(tab.name);
            
            // Play tab change SFX.
            m_soundContainers[2].Play();
        }

        /* Method for the unselected tab: 
           -  Takes a tab as a parameter and removes the currentlySelectedTab class
           -  Then finds the tab content and adds the unselectedContent class */
        private void UnselectTab(Button tab)
        {
            tab.RemoveFromClassList(currentlySelectedTabClassName);
            ListView content = FindContent(tab);
            content.AddToClassList(unselectedContentClassName);
        }

        // Method to generate the associated tab content name by for the given tab name
        private static string GenerateContentName(Button tab) =>
            tab.name.Replace(tabNameSuffix, contentNameSuffix);

        // Method that takes a tab as a parameter and returns the associated content element
        private ListView FindContent(Button tab)
        {
            return (ListView)(root.Q(name: GenerateContentName(tab)));
        }

        // Sets up the item binding for the inventory UI.
        private void InstantiateJar<T>(ListView jar) where T : struct
        {
            // Set up a make item function for a list entry.
            jar.makeItem = () =>
            {
                // Instantiate the UXML template for the entry
                var newListEntry = listEntryTemplate.Instantiate();

                // Instantiate a controller for the data
                var newListEntryLogic = new TabbedInventoryItemController(newListEntry.Q<Button>(), OnItemEntryClicked);

                // Assign the controller script to the visual element
                newListEntry.userData = newListEntryLogic;

                // Return the root of the instantiated visual tree
                return newListEntry;
            };

            SetItemSource<T>(ref jar);

            // Set up bind function for a specific list entry.
            jar.bindItem = (item, index) =>
            {
                // For now, this doesn't do anything.
                // We can connect this to the Sprite stored in the struct (?) or the collections item later.
                (item.userData as TabbedInventoryItemController).SetInventoryData((IInventoryItem)(jar.itemsSource[index]));
            };

            // Set a fixed item height
            jar.fixedItemHeight = 100;
        }

        // Instantiate a new item for the inventory.
        public void RebuildJar(IInventoryItem item)
        {
            ListView jar = null;
            switch (item.itemType)
            {
                case ItemType.Tool:
                    jar = m_contentJars.Find(jar => jar.name == "tools" + contentNameSuffix);
                    SetItemSource<GrandmaGreen.Collections.Tool>(ref jar);
                    break;
                case ItemType.Seed:
                    jar = m_contentJars.Find(jar => jar.name == "seeds" + contentNameSuffix);
                    SetItemSource<Seed>(ref jar);
                    break;
                case ItemType.Plant:
                    jar = m_contentJars.Find(jar => jar.name == "plants" + contentNameSuffix);
                    SetItemSource<Plant>(ref jar);
                    break;
                case ItemType.Decor:
                    jar = m_contentJars.Find(jar => jar.name == "decor" + contentNameSuffix);
                    SetItemSource<Decor>(ref jar);
                    break;
            }

            if (jar != null)
            {
                jar.Rebuild();
            }
        }

        private void SetItemSource<T>(ref ListView jar) where T : struct
        {
            // Set the actual item's source list/array
            foreach (IComponentStore componentStore in m_inventoryData.componentStores)
            {
                if (componentStore.GetType() == typeof(T))
                {
                    jar.itemsSource = ((ComponentStore<T>)componentStore).components;
                }
            }
        }

        public void OnItemEntryClicked(int itemID)
        {
            playerToolData.SetEquippedPlant(itemID);
            onItemEntryClicked?.Invoke(itemID);
            if(itemID != null){
                CloseInventory(new ClickEvent());
            }
        }

        void CheckOpenInventory(ToolData selectedTool)
        {
            if (selectedTool.toolIndex == 3)
                OpenInventory(); 
                Button seedsTab = root.Q<Button>("seeds-tab");
                GetAllTabs().Where(
                    (tab) => tab != seedsTab && TabIsCurrentlySelected(tab)
                ).ForEach(UnselectTab);
                SelectTab(seedsTab);
        }
    }
}