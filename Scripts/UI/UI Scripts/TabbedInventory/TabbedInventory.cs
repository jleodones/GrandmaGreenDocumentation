// This script attaches the tabbed menu logic to the game.

using System.Collections;
using System.Collections.Generic;
using Core.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using Core.Input;
using GrandmaGreen.SaveSystem;
using GrandmaGreen.Collections;
using GrandmaGreen.Garden;
using SpookuleleAudio;

namespace GrandmaGreen.UI.Collections
{
    public enum InventoryMode
    {
        Default,
        Customization,
        Selling,
        Gifting
    }
    
    public class TabbedInventory : UIDisplayBase, IDraggableContainer
    {
        public const string contentContainerNameSuffix = "ContentContainer";
        public const string contentNameSuffix = "Content";

        public PlayerToolData playerToolData;

        // Check scene.
        public InventoryMode currentMode = InventoryMode.Customization;
        
        // Template for list items.
        public VisualTreeAsset listEntryTemplate;
        
        // Inventory scriptable object with all inventory data.
        public ObjectSaver inventoryData;

        private TabbedInventoryController m_controller;

        public ASoundContainer[] soundContainers;

        // For shopping mode.
        public SellingUIDisplay sellingUI;
        
        // Related to being a draggable container.
        public VisualElement threshold { get; set; }
        public VisualElement content { get; set; }
        public Vector3 pointerStartPosition { get; set; }

        public bool handled { get; set; }
        public IDraggable draggable { get; set; }

        // Dragging UI objects
        [SerializeField] Canvas dragUI;
        private UnityEngine.UI.Image itemSprite;
        [SerializeField] PointerState pointerState;

        void Start()
        {
            // Register player tab clicking events.
            RegisterTabCallbacks();
            playerToolData.onRequireSeedEquip += CheckOpenInventory;

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
            content = m_rootVisualElement.Q("content");
            draggable = null;
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
                    jar.userData = (ComponentStore<T>)componentStore;
                    break;
                }
            }
        }

        void InstantiateJar<T>(VisualElement jar) where T : struct
        {
            SetItemSource<T>(ref jar);
            BuildJar<T>(ref jar);
        }

        void BuildJar<T>(ref VisualElement jar) where T : struct
        {
            // Clear the jar.
            jar.Clear();

            // Add a child for each item in the list.
            // Retrieve list.
            var componentStore = (ComponentStore<T>)jar.userData;

            for (int i = 0; i < componentStore.components.Count; i++)
            {
                // Instantiate the list entry.
                var newListEntry = listEntryTemplate.Instantiate();
                
                var newListEntryLogic = new TabbedInventoryItemController(newListEntry.Q<Button>());
                
                // Instantiate a controller for the data
                if (typeof(T) == new Seed().GetType())
                {
                    newListEntryLogic.SetButtonCallback(OnSeedEntryClicked);

                    var t = (ComponentStore<Seed>)jar.userData;
                    Seed s = (Seed)t.components[i];
                    Sprite sprite = CollectionsSO.LoadedInstance.GetSprite((PlantId)s.itemID, s.seedGenotype);
                    newListEntryLogic.SetInventoryData(s, sprite);
                    newListEntryLogic.SetSizeBadge(s.seedGenotype);

                    newListEntry.userData = newListEntryLogic;
                }
                else if (typeof(T) == new Plant().GetType())
                {
                    var t = (ComponentStore<Plant>)jar.userData;
                    Plant p = (Plant)t.components[i];
                    Sprite sprite = CollectionsSO.LoadedInstance.GetInventorySprite((PlantId)p.itemID, p.plantGenotype);
                    newListEntryLogic.SetInventoryData(p, sprite);
                    newListEntryLogic.SetSizeBadge(p.plantGenotype);

                    Sprite spr = CollectionsSO.LoadedInstance.GetInventorySprite((PlantId)p.itemID, p.plantGenotype);
                    newListEntryLogic.SetSizeBadge(p.plantGenotype);

                    newListEntry.userData = newListEntryLogic;
                }
                else
                {
                    IInventoryItem item = (IInventoryItem)componentStore.components[i];
                    Sprite sprite = CollectionsSO.LoadedInstance.GetSprite(item.itemID);
                    newListEntryLogic.SetInventoryData(item, sprite);

                    newListEntry.userData = newListEntryLogic;
                }

                
                OnItemCreated(newListEntryLogic);
                
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

        public void RebuildAllJars()
        {
            VisualElement jar;
            // First, get all content jars.
            List<VisualElement> contentJars = GetAllContentJars().ToList();
            
            jar = contentJars.Find(jar => jar.name == "tools" + contentNameSuffix);
            BuildJar<Tool>(ref jar);

            jar = contentJars.Find(jar => jar.name == "seeds" + contentNameSuffix);
            BuildJar<Seed>(ref jar);
            
            jar = contentJars.Find(jar => jar.name == "plants" + contentNameSuffix);
            BuildJar<Plant>(ref jar);
            
            jar = contentJars.Find(jar => jar.name == "decor" + contentNameSuffix);
            BuildJar<Decor>(ref jar);
        }

        public void OnSeedEntryClicked(TabbedInventoryItemController itemController)
        {
            if (currentMode == InventoryMode.Customization)
            {
                Seed s = (Seed)itemController.inventoryItemData;

                playerToolData.SetEquippedSeed(s.itemID, s.seedGenotype);
                CloseUI();
            }
        }

        public void OnItemCreated(TabbedInventoryItemController itemController)
        {
            itemController.button.RegisterCallback<PointerDownEvent>(PointerDownHandler, TrickleDown.TrickleDown);

            // TODO: Because we are moving the sprites instead, we also want to get rid of the callbacks for this pointer move handler.
            itemController.button.RegisterCallback<PointerMoveEvent>(PointerMoveHandler, TrickleDown.TrickleDown);

            // TODO: Test whether the pointer up handler still works with the sprite method.
            itemController.button.RegisterCallback<PointerUpEvent>(PointerUpHandler, TrickleDown.TrickleDown);
        }

        public void PointerDownHandler(PointerDownEvent evt)
        {
            // Retrieve button.
            Button draggedButton = evt.currentTarget as Button;
            Canvas canvas = Instantiate(dragUI);

            // TODO: Turn it into a draggable sprite.
            // Retrieve the inventory sprite object from the draggedButton.userData as TabbedInventoryItemController.

            // Instantiate a sprite.
            // Capture the pointer on that sprite so that the sprite moves on drag instead of the inventory item.
            // Meanwhile, hide the draggedButton so that it appears to be "moving."
            StartCoroutine(DragItem(draggedButton, canvas));

            //draggable = draggedButton.parent.userData as IDraggable;
            //draggable.startingPosition = Vector3.zero;

            //pointerStartPosition = evt.position;
            //draggable.button.CapturePointer(evt.pointerId);
            handled = false;
        }

        public IEnumerator DragItem(Button draggedButton, Canvas canvas)
        {
            itemSprite = canvas.GetComponentInChildren<UnityEngine.UI.Image>();

            TabbedInventoryItemController item = draggedButton.parent.userData as TabbedInventoryItemController;
            Sprite sprite = item.sprite;
            item.SetAlpha(0.2f);

            ItemType type = item.inventoryItemData.itemType;
            //if (type != ItemType.Decor) yield break;


            itemSprite.sprite = sprite;

            float scale = m_rootVisualElement.resolvedStyle.width / (float)Screen.width;

            do
            {
                itemSprite.transform.position = pointerState.positionRaw;

                if (scale * itemSprite.transform.position.x <= threshold.worldTransform.GetPosition().x && !handled)
                {
                    CloseUI();

                    EventManager.instance.HandleEVENT_INVENTORY_CUSTOMIZATION_START(item.inventoryItemData);

                    Destroy(canvas.gameObject);
                    itemSprite = null;
                    item.SetAlpha(1.0f);

                    handled = true;
                }

                yield return null;

            } while (!handled && pointerState.phase != PointerState.Phase.NONE);

            if (!handled)
            {
                Destroy(canvas.gameObject);
                itemSprite = null;
                item.SetAlpha(1.0f);
            }
        }

        public void PointerMoveHandler(PointerMoveEvent evt)
        {
            switch (currentMode)
            {
                case InventoryMode.Default:
                    break;
                case InventoryMode.Gifting:
                    break;
                case InventoryMode.Customization:
                    CustomizationDrag(evt);
                    break;
                case InventoryMode.Selling:
                    SellingDrag(evt);
                    break;
            }
        }

        private void SellingDrag(PointerMoveEvent evt)
        {
            if (draggable != null && draggable.button.HasPointerCapture(evt.pointerId))
            {
                Vector3 pointerDelta = evt.position - pointerStartPosition;
                draggable.button.transform.position = new Vector2(draggable.startingPosition.x + pointerDelta.x, draggable.startingPosition.y + pointerDelta.y);

                // Threshold Check
                if(sellingUI.IsInBounds(draggable.button.worldTransform.GetPosition()) && !handled){
                    FinishPointer(evt.pointerId);
                    handled = true;
                    
                    // Pass off the information to the selling box so it can instantiate the object.
                    // UI to GameObject here
                    VisualElement ve = ((Button) evt.currentTarget).parent;
                    IInventoryItem item = (ve.userData as TabbedInventoryItemController).inventoryItemData;
                    sellingUI.AddItem(item);
                }
            }
        }

        private void CustomizationDrag(PointerMoveEvent evt)
        {
            // UI to GameObject here
            VisualElement ve = ((Button) evt.currentTarget).parent;
            IInventoryItem item = (ve.userData as TabbedInventoryItemController).inventoryItemData;

            if (item.itemType != ItemType.Decor)
            {
                return;
            }
            
            if (draggable != null && draggable.button.HasPointerCapture(evt.pointerId))
            {
                Vector3 pointerDelta = evt.position - pointerStartPosition;
                draggable.button.transform.position = new Vector2(draggable.startingPosition.x + pointerDelta.x, draggable.startingPosition.y + pointerDelta.y);

                // To-Do: Bound Checks
                // Threshold Check
                if (draggable.button.worldTransform.GetPosition().x <= threshold.worldTransform.GetPosition().x && !handled)
                {
                    FinishPointer(evt.pointerId);
                    CloseUI();
                    
                    EventManager.instance.HandleEVENT_INVENTORY_CUSTOMIZATION_START(item);
                    handled = true;
                }
            }
        }

        public void PointerUpHandler(PointerUpEvent evt)
        {
            FinishPointer(evt.pointerId);
        }

        public void FinishPointer(int pointerId)
        {
            if (draggable != null && draggable.button.HasPointerCapture(pointerId))
            {
                draggable.button.ReleasePointer(pointerId);
                draggable.button.transform.position = Vector3.zero;
                draggable = null;
            }
        }
        
        void CheckOpenInventory()
        {

            // Open inventory with a short delay for tool animation
            m_rootVisualElement.style.transitionDelay = new List<TimeValue> { new(700, TimeUnit.Millisecond) };
            OpenUI();
            m_rootVisualElement.style.transitionDelay = new List<TimeValue> { new(0, TimeUnit.Millisecond) };
            Button seedsTab = m_rootVisualElement.Q<Button>("seeds-tab");
            GetAllTabs().Where(
                (tab) => tab != seedsTab && tab.ClassListContains("active-tab")
            ).ForEach(UnselectTab);
            SelectTab(seedsTab);

        }

        // LOADING AND UNLOADING UI.
        public override void Load()
        {
            // Sets up the controller for the whole inventory. The controller instantiates the inventory on its own upon creation.
            m_controller = new(this, inventoryData);

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