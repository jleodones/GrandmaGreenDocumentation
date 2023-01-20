// This script attaches the tabbed menu logic to the game.

using System;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.SaveSystem;
using GrandmaGreen.Collections;
using GrandmaGreen.Garden;
using SpookuleleAudio;

namespace GrandmaGreen.UI.Collections
{
    public class TabbedInventory : UIDisplayBase, IDraggableContainer
    {
        public const string contentContainerNameSuffix = "ContentContainer";
        public const string contentNameSuffix = "Content";

        public PlayerToolData playerToolData;

        // Template for list items.
        public VisualTreeAsset listEntryTemplate;

        // Template for list items.
        public VisualTreeAsset infoListEntryTemplate;
        // Collections SO.
        public CollectionsSO collectionsSO;

        // Inventory scriptable object with all inventory data.
        public ObjectSaver inventoryData;
        
        private TabbedInventoryController m_controller;

        public ASoundContainer[] soundContainers;

        // Related to being a draggable container.
        public VisualElement threshold { get; set; }
        public Vector3 pointerStartPosition { get; set; }

        public bool enabled { get; set; }
        public bool handled { get; set; }
        
        public IDraggable draggable { get; set; }
        void Start()
        {
            // Register player tab clicking events.
            RegisterTabCallbacks();
            
            playerToolData.onToolSelected += CheckOpenInventory;
            
            // Threshold.
            m_rootVisualElement.Q("inventory-threshold");
            
            // Instantiate the inventory items.
            // First, get all content jars.
            List<VisualElement> contentJars = GetAllContentJars().ToList();

            // Manually instantiate each jar.
            InstantiateJar<Tool>(contentJars.Find(jar => jar.name == "tools" + contentNameSuffix));
            InstantiateJar<Seed>(contentJars.Find(jar => jar.name == "seeds" + contentNameSuffix));
            InstantiateJar<Decor>(contentJars.Find(jar => jar.name == "decor" + contentNameSuffix));
            InstantiateJar<Plant>(contentJars.Find(jar => jar.name == "plants" + contentNameSuffix));
            
            // Threshold instantiation.
            threshold = m_rootVisualElement.Q("inventory-threshold");
        }

        // OPENING AND CLOSING THE UI.
        public override void OpenUI()
        {
            // Close HUD first.
            EventManager.instance.HandleEVENT_CLOSE_HUD();
            base.OpenUI();
            // Play open inventory SFX.
            soundContainers[0].Play();
        }

        public override void CloseUI()
        {
            // Play the inventory close SFX.
            soundContainers[1].Play();
            base.CloseUI();
            EventManager.instance.HandleEVENT_OPEN_HUD();
        }

        // BUILDING THE TABS SWITCHING EVENTS.
        UQueryBuilder<Button> GetAllTabs()
        {
            return m_rootVisualElement.Query<Button>(className: "tab-button");
        }

        private ScrollView FindContent(Button tab)
        {
            string contentName = tab.name.Replace("-tab", contentContainerNameSuffix);
            return (ScrollView)(m_rootVisualElement.Q(name: contentName));
        }
        
        void SelectTab(Button tab)
        {
            tab.AddToClassList("active-tab");

            ScrollView content = FindContent(tab);
            content.RemoveFromClassList("inactive-content");
            
            // Play tab change SFX.
            soundContainers[2].Play();
        }

        void UnselectTab(Button tab)
        {
            tab.RemoveFromClassList("active-tab");
            
            ScrollView content = FindContent(tab);
            content.AddToClassList("inactive-content");
        }
        
        void RegisterTabCallbacks()
        {
            // Get the list of tabs.
            UQueryBuilder<Button> tabs = GetAllTabs();
            tabs.ForEach((Button tab) =>
            {
                tab.RegisterCallback<ClickEvent>(evt =>
                {
                    Button clickedTab = evt.currentTarget as Button;
                    // If tab is not the active tab, mark it as the active tab.
                    if (!tab.ClassListContains("active-tab"))
                    {
                        // Marking everything else as inactive.
                        GetAllTabs().Where(
                            (tab) => tab != clickedTab && tab.ClassListContains("active-tab")
                        ).ForEach(UnselectTab);
                        
                        // Marking this tab as active.
                        SelectTab(clickedTab);
                    }
                });
            });
        }
        
        // BUILDING CONTENT JARS.
        UQueryBuilder<VisualElement> GetAllContentJars()
        {
            return m_rootVisualElement.Query<VisualElement>(className: "inventory-content");
        }

        void SetItemSource<T>(ref VisualElement jar) where T : struct
        {
            foreach (IComponentStore componentStore in inventoryData.componentStores)
            {
                if (componentStore.GetType() == typeof(T))
                {
                    jar.userData = (ComponentStore<T>) componentStore;
                    break;
                }
            }
        }

        void InstantiateJar<T>(VisualElement jar) where T : struct
        {
            SetItemSource<T>(ref jar);
            BuildJar<T>(ref jar);

            Rect jarBound = jar.localBound;
            Rect parentBound = jar.parent.localBound;
        }

        void BuildJar<T>(ref VisualElement jar) where T : struct
        {
            // Clear the jar.
            jar.Clear();
            
            // Add a child for each item in the list.
            // Retrieve list.
            var componentStore = (ComponentStore<T>) jar.userData;
            
            for (int i = 0; i < componentStore.components.Count; i++)
            {
                // Instantiate the list entry.
                var newListEntry = listEntryTemplate.Instantiate();
                
                // Instantiate a controller for the data
                if (typeof(T) == new Seed().GetType())
                {
                    var newListEntryLogic =
                        new TabbedInventoryItemController(newListEntry.Q<Button>());
                    newListEntryLogic.SetButtonCallback(OnSeedEntryClicked);

                    var t = (ComponentStore<Seed>)jar.userData;
                    Seed s = (Seed)t.components[i];
                    Sprite sprite = collectionsSO.GetSprite((PlantId)s.itemID, s.seedGenotype);
                    newListEntryLogic.SetInventoryData(s, sprite);
                    newListEntryLogic.SetSizeBadge(s.seedGenotype);
                    
                    newListEntry.userData = newListEntryLogic;
                }
                else if (typeof(T) == new Plant().GetType())
                {
                    var newListEntryLogic =
                        new TabbedInventoryItemController(newListEntry.Q<Button>());

                    var t = (ComponentStore<Plant>)jar.userData;
                    Plant p = (Plant) t.components[i];
                    Sprite sprite = collectionsSO.GetInventorySprite((PlantId)p.itemID, p.plantGenotype);
                    newListEntryLogic.SetInventoryData(p, sprite);
                    newListEntryLogic.SetSizeBadge(p.plantGenotype);

                    Sprite spr = collectionsSO.GetInventorySprite((PlantId)p.itemID, p.plantGenotype);
                    newListEntryLogic.SetSizeBadge(p.plantGenotype);
                    
                    newListEntry.userData = newListEntryLogic;
                }
                else
                {
                    var newListEntryLogic = new TabbedInventoryItemController(newListEntry.Q<Button>());

                    IInventoryItem item = (IInventoryItem)componentStore.components[i];
                    Sprite sprite = collectionsSO.GetSprite(item.itemID);
                    newListEntryLogic.SetInventoryData(item, sprite);

                    if (typeof(T) == new Decor().GetType())
                    {
                        OnItemCreated(newListEntryLogic);
                    }

                    newListEntry.userData = newListEntryLogic;
                }
                
                // Append to jar.
                jar.Add(newListEntry);
            }
        }
        
        // Instantiate a new item for the inventory.
        public void RebuildJar(IInventoryItem item)
        {
            VisualElement jar = null;
            // First, get all content jars.
            List<VisualElement> contentJars = GetAllContentJars().ToList();
            
            switch (item.itemType)
            {
                case ItemType.Tool:
                    jar = contentJars.Find(jar => jar.name == "tools" + contentNameSuffix);
                    BuildJar<Tool>(ref jar);
                    break;
                case ItemType.Seed:
                    jar = contentJars.Find(jar => jar.name == "seeds" + contentNameSuffix);
                    BuildJar<Seed>(ref jar);
                    break;
                case ItemType.Plant:
                    jar = contentJars.Find(jar => jar.name == "plants" + contentNameSuffix);
                    BuildJar<Plant>(ref jar);
                    break;
                case ItemType.Decor:
                    jar = contentJars.Find(jar => jar.name == "decor" + contentNameSuffix);
                    BuildJar<Decor>(ref jar);
                    break;
            }
        }

        public void OnSeedEntryClicked(TabbedInventoryItemController itemController)
        {
            Seed s = (Seed) itemController.m_inventoryItemData;

            playerToolData.SetEquippedSeed(s.itemID, s.seedGenotype);
            CloseUI();
        }

        public void OnItemCreated(TabbedInventoryItemController itemController)
        {
            itemController.button.RegisterCallback<PointerDownEvent>(PointerDownHandler, TrickleDown.TrickleDown);
            itemController.button.RegisterCallback<PointerMoveEvent>(PointerMoveHandler, TrickleDown.TrickleDown);
            itemController.button.RegisterCallback<PointerUpEvent>(PointerUpHandler, TrickleDown.TrickleDown);
        }
        
        public void PointerDownHandler(PointerDownEvent evt)
        {
            Button draggedButton = evt.currentTarget as Button;
            draggable = draggedButton.parent.userData as IDraggable;
            draggable.startingPosition = Vector3.zero;
            
            pointerStartPosition = evt.position;
            draggable.button.CapturePointer(evt.pointerId);
            enabled = true;
            handled = false;
        }

        public void PointerMoveHandler(PointerMoveEvent evt)
        {
            if (enabled && draggable.button.HasPointerCapture(evt.pointerId))
            {
                Vector3 pointerDelta = evt.position - pointerStartPosition;
                draggable.button.transform.position = new Vector2(draggable.startingPosition.x + pointerDelta.x, draggable.startingPosition.y + pointerDelta.y);

                // To-Do: Bound Checks
                // Threshold Check
                if(draggable.button.worldTransform.GetPosition().x <= threshold.worldTransform.GetPosition().x && !handled){
                    draggable.button.transform.position = Vector3.zero;
                    draggable.button.ReleasePointer(evt.pointerId);
                    CloseUI();
                    
                    // UI to GameObject here
                    VisualElement ve = ((Button) evt.currentTarget).parent;
                    IInventoryItem item = (ve.userData as TabbedInventoryItemController).m_inventoryItemData;
                    EventManager.instance.HandleEVENT_INVENTORY_CUSTOMIZATION_START(item);
                    handled = true;
                }
            }
        }

        public void PointerUpHandler(PointerUpEvent evt)
        {
            if (enabled && draggable.button.HasPointerCapture(evt.pointerId))
            {
                draggable.button.ReleasePointer(evt.pointerId);
                draggable.button.transform.position = Vector3.zero;
            }
        }

        void CheckOpenInventory(ToolData selectedTool)
        {
            if (selectedTool.toolIndex == 3)
            {
                // Open inventory with a short delay for tool animation
                m_rootVisualElement.style.transitionDelay = new List<TimeValue> {new(700, TimeUnit.Millisecond) };
                OpenUI();
                m_rootVisualElement.style.transitionDelay = new List<TimeValue> {new(0, TimeUnit.Millisecond) };
                Button seedsTab = m_rootVisualElement.Q<Button>("seeds-tab");
                GetAllTabs().Where(
                    (tab) => tab != seedsTab && tab.ClassListContains("active-tab")
                ).ForEach(UnselectTab);
                SelectTab(seedsTab);
            }
        }

        // LOADING AND UNLOADING UI.
        public override void Load()
        {
            // Sets up the controller for the whole inventory. The controller instantiates the inventory on its own upon creation.
            m_controller = new(this, inventoryData, collectionsSO);
            
            // Subscribe to inventory related events.
            EventManager.instance.EVENT_INVENTORY_OPEN += OpenUI;

            EventManager.instance.EVENT_INVENTORY_ADD_PLANT += m_controller.InventoryAddPlant;
            EventManager.instance.EVENT_INVENTORY_REMOVE_PLANT += m_controller.InventoryRemovePlant;

            EventManager.instance.EVENT_INVENTORY_ADD_SEED += m_controller.InventoryAddSeed;
            EventManager.instance.EVENT_INVENTORY_REMOVE_SEED += m_controller.InventoryRemoveSeed;
            EventManager.instance.EVENT_INVENTORY_GET_SEED_COUNT += m_controller.InventoryGetSeedCount;

            EventManager.instance.EVENT_INVENTORY_ADD_TOOL += m_controller.InventoryAddTool;
            EventManager.instance.EVENT_INVENTORY_REMOVE_TOOL += m_controller.InventoryRemoveTool;

            EventManager.instance.EVENT_INVENTORY_ADD_DECOR += m_controller.InventoryAddDecor;
            EventManager.instance.EVENT_INVENTORY_REMOVE_DECOR += m_controller.InventoryRemoveDecor;

            // Money.
            EventManager.instance.EVENT_INVENTORY_ADD_MONEY += m_controller.InventoryAddMoney;
            EventManager.instance.EVENT_INVENTORY_REMOVE_MONEY += m_controller.InventoryRemoveMoney;
            EventManager.instance.EVENT_INVENTORY_GET_MONEY += m_controller.InventoryGetMoney;
        }

        public override void Unload()
        {
            // Unsubscribe to inventory related events.
            EventManager.instance.EVENT_INVENTORY_OPEN -= OpenUI;

            EventManager.instance.EVENT_INVENTORY_ADD_PLANT -= m_controller.InventoryAddPlant;
            EventManager.instance.EVENT_INVENTORY_REMOVE_PLANT -= m_controller.InventoryRemovePlant;

            EventManager.instance.EVENT_INVENTORY_ADD_SEED -= m_controller.InventoryAddSeed;
            EventManager.instance.EVENT_INVENTORY_REMOVE_SEED -= m_controller.InventoryRemoveSeed;
            EventManager.instance.EVENT_INVENTORY_GET_SEED_COUNT -= m_controller.InventoryGetSeedCount;

            EventManager.instance.EVENT_INVENTORY_ADD_TOOL -= m_controller.InventoryAddTool;
            EventManager.instance.EVENT_INVENTORY_REMOVE_TOOL -= m_controller.InventoryRemoveTool;

            EventManager.instance.EVENT_INVENTORY_ADD_DECOR -= m_controller.InventoryAddDecor;
            EventManager.instance.EVENT_INVENTORY_REMOVE_DECOR -= m_controller.InventoryRemoveDecor;

            // Money.
            EventManager.instance.EVENT_INVENTORY_ADD_MONEY -= m_controller.InventoryAddMoney;

            EventManager.instance.EVENT_INVENTORY_REMOVE_MONEY -= m_controller.InventoryRemoveMoney;

            EventManager.instance.EVENT_INVENTORY_GET_MONEY -= m_controller.InventoryGetMoney;
        }
    }
}