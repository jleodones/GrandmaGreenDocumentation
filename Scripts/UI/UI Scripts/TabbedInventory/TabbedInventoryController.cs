// This script defines the tab selection logic.
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using GrandmaGreen.SaveSystem;
using GrandmaGreen.Collections;
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

        private Sprite favoritedBackground;
        
        // Entry template used to spawn info list items.
        private VisualTreeAsset infoListEntryTemplate;

        // Holds all content jars.
        private List<ListView> m_contentJars;

        // Inventory data.
        private ObjectSaver m_inventoryData;

        private ASoundContainer[] m_soundContainers;
        
        
        // Inventory width varibles for show/hide
        private Length inventoryWidth = (Length)(.45 * Screen.width);
        // Customization variables
        private VisualElement threshold; 
        private Button draggable;
        private Vector3 draggableStartPos;
        private IInventoryItem m_inventoryItemId;
        private CollectionsSO m_collections;

        /// <summary>
        /// The TabbedInventoryController is attached to the TabbedInventory UI. It registers and controlls tab switching, as well as loading old and new items into the inventory.
        /// </summary>
        public TabbedInventoryController(VisualElement _root, PlayerToolData _playerToolData,
            ObjectSaver inventoryData, VisualTreeAsset _listEntryTemplate, Sprite _favoritedBackground, VisualTreeAsset _infoListEntryTemplate, ASoundContainer[] soundContainers, CollectionsSO cso)
        {
            // Set member variables.
            root = _root;
            playerToolData = _playerToolData;
            listEntryTemplate = _listEntryTemplate;
            favoritedBackground = _favoritedBackground;
            infoListEntryTemplate = _infoListEntryTemplate;
            m_inventoryData = inventoryData;
            m_soundContainers = soundContainers;
            m_collections = cso;

            // Instantiate the inventory items.
            // First, get all content jars.
            m_contentJars = GetAllContentJars().ToList();

            // Manually instantiate each jar.
            InstantiateJar<Tool>(m_contentJars.Find(jar => jar.name == "tools" + contentNameSuffix));
            InstantiateJar<Seed>(m_contentJars.Find(jar => jar.name == "seeds" + contentNameSuffix));
            InstantiateJar<Decor>(m_contentJars.Find(jar => jar.name == "decor" + contentNameSuffix));
            InstantiateJar<Plant>(m_contentJars.Find(jar => jar.name == "plants" + contentNameSuffix));

            _playerToolData.onToolSelected += CheckOpenInventory;

            // Instantiate customization drag threshold
            threshold = root.Q("inventory-threshold");
        }

        /*
        // This hides the entire inventory panel on initialize without animations
        public void SetInventoryPosition()
        {
            // Add animation duration
            // inventory.style.translate = new Translate(inventoryWidth,0,0);
            root.style.display = DisplayStyle.None;
        }
        */

        public void RegisterTabCallbacks()
        {
            UQueryBuilder<Button> tabs = GetAllTabs();
            tabs.ForEach((Button tab) => { tab.RegisterCallback<ClickEvent>(TabOnClick); });
        }

        public void OpenInventory()
        {
            // Disable HUD.
            EventManager.instance.HandleEVENT_CLOSE_HUD_ANIMATED();

            // inventory.style.transitionProperty = new List<StylePropertyName> { "translate" };
            // inventory.style.transitionTimingFunction = new List<EasingFunction> { EasingMode.Ease };
            // inventory.style.transitionDuration = new List<TimeValue>{ new TimeValue(300, TimeUnit.Millisecond) };
            // inventory.style.translate = new Translate(0,0,0);
            root.style.display = DisplayStyle.Flex;
            
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
            root.style.display = DisplayStyle.None;

            // Play the inventory close SFX.
            m_soundContainers[1].Play();
            
            root.Q<VisualElement>("infoWindowContainer").style.display = DisplayStyle.None;

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
            SetItemSource<T>(ref jar);

            // Set up a make item function for a list entry.
            jar.makeItem = () =>
            {
                // Instantiate the UXML template for the entry
                var newListEntry = listEntryTemplate.Instantiate();

                // Instantiate a controller for the data
                if (typeof(T) == new Seed().GetType())
                {
                    var newListEntryLogic =
                        new TabbedInventoryItemController(newListEntry.Q<Button>());
                    newListEntryLogic.SetButtonCallback(OnSeedEntryClicked);
                    newListEntry.userData = newListEntryLogic;
                }
                else if (typeof(T) == new Plant().GetType())
                {
                    var newListEntryLogic =
                        new TabbedInventoryItemController(newListEntry.Q<Button>());
                    newListEntryLogic.SetButtonCallback(OpenInformationPopup);
                    newListEntry.userData = newListEntryLogic;
                }
                else
                {
                    var newListEntryLogic = new TabbedInventoryItemController(newListEntry.Q<Button>());
                    newListEntryLogic.SetButtonCallback(OnDecorItemClicked);
                    newListEntry.userData = newListEntryLogic;
                }

                // Return the root of the instantiated visual tree
                return newListEntry;
            };
            
            // Set up bind function for a specific list entry.
            jar.bindItem = (item, index) =>
            {
                // Get the item image.
                Sprite sprite;

                // Instantiate a controller for the data
                if (typeof(T) == new Seed().GetType())
                {
                    Seed s = (Seed)(jar.itemsSource[index]);
                    sprite = m_collections.GetSprite((PlantId)s.itemID, s.seedGenotype);

                    (item.userData as TabbedInventoryItemController).SetInventoryData(s, sprite);
                }
                else if (typeof(T) == new Plant().GetType())
                {
                    Plant p = (Plant)(jar.itemsSource[index]);
                    sprite = m_collections.GetSprite((PlantId)p.itemID, p.genotypes[0], 2);
                    (item.userData as TabbedInventoryItemController).SetInventoryData(
                        (IInventoryItem)(jar.itemsSource[index]), sprite);

                    if (p.isFavorited)
                    {
                        (item.userData as TabbedInventoryItemController).SetFavorite(favoritedBackground);
                    }
                }
                else
                {
                    IInventoryItem i = (IInventoryItem)(jar.itemsSource[index]);
                    sprite = m_collections.GetSprite(i.itemID);
                    (item.userData as TabbedInventoryItemController).SetInventoryData(
                        (IInventoryItem)(jar.itemsSource[index]), sprite);
                    OnItemCreated(item.userData as TabbedInventoryItemController);
                }
            };
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
                if (jar.visible)
                {
                    jar.RefreshItems();
                }
                else
                {
                    jar.Rebuild();
                }
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
                    break;
                }
            }
        }

        public void OnSeedEntryClicked(TabbedInventoryItemController itemController)
        {
            Seed s = (Seed) itemController.m_inventoryItemData;

            playerToolData.SetEquippedSeed(s.itemID, s.seedGenotype);
            CloseInventory(new ClickEvent());
        }

        // TODO: Need to rework the customization system. Currently it sets the m_inventoryItemID to the last item created.
        // OnDecorItemClicked is a stopgap measure put in place to prevent it, but needs to be reworked.
        public void OnDecorItemClicked(TabbedInventoryItemController itemController)
        {
            m_inventoryItemId = itemController.m_inventoryItemData;
        }

        public void OnItemCreated(TabbedInventoryItemController itemController)
        {
            // m_inventoryItemId = itemController.m_inventoryItemData;
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

                    VisualElement ve = ((Button) evt.currentTarget).parent;
                    IInventoryItem item = (ve.userData as TabbedInventoryItemController).m_inventoryItemData;
                    EventManager.instance.HandleEVENT_INVENTORY_CUSTOMIZATION_START(item);
                    handled = true;
                }
            }
        }

        private void PointerUpHandler(PointerUpEvent evt)
        {
            if (enabled && draggable.HasPointerCapture(evt.pointerId))
            {
                draggable.ReleasePointer(evt.pointerId);
                draggable.transform.position = targetStartPosition;
            }
        }

        void CheckOpenInventory(ToolData selectedTool)
        {
            if (selectedTool.toolIndex == 3)
            {
                // Open inventory with a short delay for tool animation
                root.style.transitionDelay = new List<TimeValue> {new(700, TimeUnit.Millisecond) };
                OpenInventory(); 
                root.style.transitionDelay = new List<TimeValue> {new(0, TimeUnit.Millisecond) };
                Button seedsTab = root.Q<Button>("seeds-tab");
                GetAllTabs().Where(
                    (tab) => tab != seedsTab && TabIsCurrentlySelected(tab)
                ).ForEach(UnselectTab);
                SelectTab(seedsTab);
            }
        }

        // Specifically for plants.
        void OpenInformationPopup(TabbedInventoryItemController itemController)
        {
            // Get the plant item.
            Plant p = (Plant) itemController.m_inventoryItemData;
            
            // SETTING UP THE INFORMATION POPUP LIST VIEW.
            // Find the information popup list view.
            ListView infoJar = root.Q<ListView>("infoWindowListView");

            // Set up a make item function for a list entry.
            infoJar.makeItem = () =>
            {
                // Instantiate the UXML template for the entry
                var newListEntry = infoListEntryTemplate.Instantiate();

                // Return the root of the instantiated visual tree
                return newListEntry;
            };
            
            // Set up bind function for a specific list entry.
            infoJar.bindItem = (item, index) =>
            {
                // Get this particular plant.
                Genotype genotype = (Genotype) infoJar.itemsSource[index];
                
                // Get and set the item image.
                Sprite sprite = m_collections.GetSprite((PlantId) p.itemID, genotype, 2);
                item.Q<VisualElement>("plantImage").style.backgroundImage = new StyleBackground(sprite);
                
                // Get the button and set the callback.
                item.Q<Button>("plantFavoriteButton").clicked += () =>
                {
                    if (!p.isFavorited)
                    {
                        // Remove it from this list.
                        EventManager.instance.HandleEVENT_INVENTORY_REMOVE_PLANT(p.itemID, genotype);

                        // Add it back in as a favorited item.
                        EventManager.instance.HandleEVENT_INVENTORY_ADD_PLANT(p.itemID, genotype, true);

                        infoJar.RefreshItems();
                    }
                };
            };

            infoJar.itemsSource = p.genotypes;
            
            infoJar.Rebuild();
            
            // CLOSE CALLBACK.
            root.Q<Button>("infoWindowExitButton").clicked += () =>
            {
                root.Q<VisualElement>("infoWindowContainer").style.display = DisplayStyle.None;
            };

            root.Q<VisualElement>("infoWindowContainer").style.display = DisplayStyle.Flex;
        }
    }
}