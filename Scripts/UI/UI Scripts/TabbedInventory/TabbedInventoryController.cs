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
        // Invenotry Visual Element
        private VisualElement inventory;
        // Inventory width varibles for show/hide
        private Length inventoryWidth = (Length)(.45 * Screen.width);
        // Customization variables
        private VisualElement threshold; 
        private Button draggable;
        private Vector3 draggableStartPos;
        private IInventoryItem m_inventoryItemId;
        private CollectionsSO m_collections;


        public event System.Action<ushort> onItemCreated;
        /// <summary>
        /// The TabbedInventoryController is attached to the TabbedInventory UI. It registers and controlls tab switching, as well as loading old and new items into the inventory.
        /// </summary>
        /// <param name="_root">The root of the TabbedInventory UI, which holds the different inventory panels.</param>
        /// <param name="_toolTestScript">Connects the tools tabs to the inventory UI.</param>
        /// <param name="inventoryData"></param>
        /// <param name="_listEntryTemplate"></param>
        public TabbedInventoryController(VisualElement _root, PlayerToolData _playerToolData,
            ObjectSaver inventoryData, VisualTreeAsset _listEntryTemplate, ASoundContainer[] soundContainers, CollectionsSO cso)
        {
            // Set member variables.
            root = _root;
            playerToolData = _playerToolData;
            listEntryTemplate = _listEntryTemplate;
            m_inventoryData = inventoryData;
            m_soundContainers = soundContainers;
            m_collections = cso;

            // Instantiate the inventory items.
            // First, get all content jars.
            m_contentJars = GetAllContentJars().ToList();

            // Manually instantiate each jar.
            InstantiateJar<GrandmaGreen.Collections.Tool>(m_contentJars.Find(jar => jar.name == "tools" + contentNameSuffix));
            InstantiateJar<Seed>(m_contentJars.Find(jar => jar.name == "seeds" + contentNameSuffix));
            InstantiateJar<Decor>(m_contentJars.Find(jar => jar.name == "decor" + contentNameSuffix));
            InstantiateJar<Plant>(m_contentJars.Find(jar => jar.name == "plants" + contentNameSuffix));

            _playerToolData.onToolSelected += CheckOpenInventory;

            // Instantiate inventory visual element
            inventory = root.Q(inventoryElement);

            // Instantiate customization drag threshold
            threshold = root.Q("inventory-threshold");
        }

        // This hides the entire inventory panel on initialize without animations
        public void SetInventoryPosition()
        {
            // Add animation duration
            // inventory.style.translate = new Translate(inventoryWidth,0,0);
            inventory.style.display = DisplayStyle.None;
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
        
        public void OpenInventory()
        {
            // Disable HUD.
            EventManager.instance.HandleEVENT_CLOSE_HUD_ANIMATED();

            // inventory.style.transitionProperty = new List<StylePropertyName> { "translate" };
            // inventory.style.transitionTimingFunction = new List<EasingFunction> { EasingMode.Ease };
            // inventory.style.transitionDuration = new List<TimeValue>{ new TimeValue(300, TimeUnit.Millisecond) };
            // inventory.style.translate = new Translate(0,0,0);
            inventory.style.display = DisplayStyle.Flex;
            
            // Play open inventory SFX.
            m_soundContainers[0].Play();
        }
        
        // This hides the entire inventory panel when the exit button is clicked
        private void CloseInventory(ClickEvent evt)
        {
            // Add animation duration
            // inventory.style.transitionProperty = new List<StylePropertyName> { "translate" };
            // inventory.style.transitionTimingFunction = new List<EasingFunction> { EasingMode.Ease };
            // inventory.style.transitionDuration = new List<TimeValue>{ new TimeValue(300, TimeUnit.Millisecond) };
            // inventory.style.translate = new Translate(inventoryWidth,0,0);
            inventory.style.display = DisplayStyle.None;

            // Play the inventory close SFX.
            m_soundContainers[1].Play();

            // Open the HUD.
            EventManager.instance.HandleEVENT_OPEN_HUD_ANIMATED();
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
                if (typeof(T) == new Seed().GetType())
                {
                    var newListEntryLogic =
                        new TabbedInventorySeedItemController(newListEntry.Q<Button>(), OnSeedEntryClicked);
                    newListEntry.userData = newListEntryLogic;
                }
                else
                {
                    var newListEntryLogic = new TabbedInventoryItemController(newListEntry.Q<Button>(), OnItemCreated);
                    newListEntry.userData = newListEntryLogic;
                }

                // Return the root of the instantiated visual tree
                return newListEntry;
            };

            SetItemSource<T>(ref jar);

            // Set up bind function for a specific list entry.
            jar.bindItem = (item, index) =>
            {
                // Get the item image.
                Sprite sprite;
                
                // Instantiate a controller for the data
                if (typeof(T) == new Seed().GetType())
                {
                    Seed s = (Seed)(jar.itemsSource[index]);
                    sprite = m_collections.GetSprite((PlantId) s.itemID, s.seedGenotype);
                    
                    (item.userData as TabbedInventorySeedItemController).SetInventoryData(s, sprite);
                }
                else if (typeof(T) == new Plant().GetType())
                {
                    Plant p = (Plant)(jar.itemsSource[index]);
                    sprite = m_collections.GetSprite((PlantId)p.itemID, p.genotypes[0], 2);
                    (item.userData as TabbedInventoryItemController).SetInventoryData((IInventoryItem)(jar.itemsSource[index]), sprite);
                }
                else
                {
                    IInventoryItem i = (IInventoryItem)(jar.itemsSource[index]);
                    sprite = m_collections.GetSprite(i.itemID);
                    (item.userData as TabbedInventoryItemController).SetInventoryData((IInventoryItem)(jar.itemsSource[index]), sprite);
                }
            };

            // Set a fixed item height
            jar.fixedItemHeight = 300;
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
                    jar.itemsSource = ((ComponentStore<T>) componentStore).components;
                }
            }
        }

        public void OnSeedEntryClicked(ushort itemID, Genotype genotype)
        {
            playerToolData.SetEquippedSeed(itemID, genotype);
            CloseInventory(new ClickEvent());
        }
        
        public void OnItemCreated(TabbedInventoryItemController itemController)
        {
            m_inventoryItemId = itemController.m_inventoryItemData;
            itemController.m_button.RegisterCallback<PointerDownEvent>(PointerDownHandler, TrickleDown.TrickleDown);
            itemController.m_button.RegisterCallback<PointerMoveEvent>(PointerMoveHandler, TrickleDown.TrickleDown);
            itemController.m_button.RegisterCallback<PointerUpEvent>(PointerUpHandler, TrickleDown.TrickleDown);
        }
        private Vector2 targetStartPosition { get; set; }
        private Vector3 pointerStartPosition { get; set; }

        private bool enabled { get; set; }
        private bool handled { get; set; }
        private void PointerDownHandler(PointerDownEvent evt)
        {
            draggable = evt.currentTarget as Button;
            draggableStartPos = draggable.transform.position;
            targetStartPosition = draggable.transform.position;
            pointerStartPosition = evt.position;
            draggable.CapturePointer(evt.pointerId);
            enabled = true;
            handled = false;
        }

        private void PointerMoveHandler(PointerMoveEvent evt)
        {
            if (enabled && draggable.HasPointerCapture(evt.pointerId))
            {
                Vector3 pointerDelta = evt.position - pointerStartPosition;
                draggable.transform.position = new Vector2(targetStartPosition.x + pointerDelta.x, targetStartPosition.y + pointerDelta.y);

                // To-Do: Bound Checks
            
                // Threshold Check
                if(draggable.worldTransform.GetPosition().x <= threshold.worldTransform.GetPosition().x && !handled){
                    draggable.transform.position = draggableStartPos;
                    CloseInventory(new ClickEvent());
                    // UI to GameObject here
                    EventManager.instance.HandleEVENT_CUSTOMIZATION_START(m_inventoryItemId);
                    handled = true;
                }
            }
        }

        private void PointerUpHandler(PointerUpEvent evt)
        {
            if (enabled && draggable.HasPointerCapture(evt.pointerId))
            {
                draggable.ReleasePointer(evt.pointerId);
                draggable.transform.position = draggableStartPos;
            }
        }

        void CheckOpenInventory(ToolData selectedTool)
        {
            if (selectedTool.toolIndex == 3)
            {
                // Open inventory with a short delay for tool animation
                inventory.style.transitionDelay = new List<TimeValue> {new(700, TimeUnit.Millisecond) };
                OpenInventory(); 
                inventory.style.transitionDelay = new List<TimeValue> {new(0, TimeUnit.Millisecond) };
                Button seedsTab = root.Q<Button>("seeds-tab");
                GetAllTabs().Where(
                    (tab) => tab != seedsTab && TabIsCurrentlySelected(tab)
                ).ForEach(UnselectTab);
                SelectTab(seedsTab);
            }
        }
    }
}