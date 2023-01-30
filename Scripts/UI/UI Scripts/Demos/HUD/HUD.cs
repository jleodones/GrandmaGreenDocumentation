// This script attaches the tabbed menu logic to the game.

using System;
using System.Collections;
using Core.SceneManagement;
using GrandmaGreen.UI.Collections;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

//Inherits from class `MonoBehaviour`. This makes it attachable to a game object as a component.
namespace GrandmaGreen.UI.HUD
{
    public class HUD : UIDisplayBase
    {
        // Cross references to other UI. Inventory handling is handled globally, as the inventory gets called opened by numerous other systems.
        // Settings.
        public SettingsUIDisplay settingsUIDisplay;

        // Cultivision.
        public CultivisionUIDisplay cultivisionUIDisplay;

        // Collection.
        public CollectionUIDisplay collectionUIDisplay;

        // Inventory.
        public TabbedInventory inventoryUIDisplay;

        private HUDController m_controller;


        public void Start()
        {
            m_controller = new(m_rootVisualElement);

            m_controller.RegisterButtonCallbacks();



            if (SceneManager.GetActiveScene().name == "SetupTest")
            {
                RegisterButtonCallbackWithClose("cultivisionButton", cultivisionUIDisplay.OpenUI);
                cultivisionUIDisplay.RegisterButtonCallback("exitButton", OpenHUDAnimated);

                RegisterButtonCallback("customizationButton", () =>
                {
                    EventManager.instance.HandleEVENT_TOGGLE_CUSTOMIZATION_MODE();
                });
            }
            else
            {
                m_rootVisualElement.Q<Button>("cultivisionButton").visible = false;
                m_rootVisualElement.Q<Button>("customizationButton").visible = false;

                m_rootVisualElement.Q<Button>("cultivisionButton").pickingMode = PickingMode.Ignore;
                m_rootVisualElement.Q<Button>("customizationButton").pickingMode = PickingMode.Ignore;
            }

            RegisterButtonCallbackWithClose("inventoryButton", inventoryUIDisplay.OpenUI);
            inventoryUIDisplay.RegisterButtonCallback("exitButton", OpenUI);

            RegisterButtonCallbackWithClose("collectionsButton", collectionUIDisplay.OpenUI);
            collectionUIDisplay.RegisterButtonCallback("exitButton", OpenHUDAnimated);

            RegisterButtonCallbackWithClose("settingsButton", settingsUIDisplay.OpenUI);
            settingsUIDisplay.RegisterButtonCallback("exitButton", OpenUI);

            EventManager.instance.HandleEVENT_UPDATE_MONEY_DISPLAY();
        }

        public override void Load()
        {
            // Register global events.
            EventManager.instance.EVENT_OPEN_HUD += OpenHUD;
            EventManager.instance.EVENT_OPEN_HUD_ANIMATED += OpenHUDAnimated;
            EventManager.instance.EVENT_CLOSE_HUD += CloseHUD;
            EventManager.instance.EVENT_CLOSE_HUD_ANIMATED += CloseHUDAnimated;
            EventManager.instance.EVENT_UPDATE_MONEY_DISPLAY += UpdateMoneyDisplay;
            EventManager.instance.EVENT_TOGGLE_CUSTOMIZATION_MODE += ToggleCustomizationMode;
        }

        public override void Unload()
        {
            // Register global events.
            EventManager.instance.EVENT_OPEN_HUD -= OpenHUD;
            EventManager.instance.EVENT_OPEN_HUD_ANIMATED -= OpenHUDAnimated;
            EventManager.instance.EVENT_CLOSE_HUD -= CloseHUD;
            EventManager.instance.EVENT_CLOSE_HUD_ANIMATED -= CloseHUDAnimated;
            EventManager.instance.EVENT_UPDATE_MONEY_DISPLAY -= UpdateMoneyDisplay;
            EventManager.instance.EVENT_TOGGLE_CUSTOMIZATION_MODE -= ToggleCustomizationMode;
        }

        void ToggleCustomizationMode()
        {
            m_rootVisualElement.Q<Button>("cultivisionButton").visible = !m_rootVisualElement.Q<Button>("cultivisionButton").visible;
            m_rootVisualElement.Q<Button>("collectionsButton").visible = !m_rootVisualElement.Q<Button>("collectionsButton").visible;

            m_rootVisualElement.Q<Button>("cultivisionButton").pickingMode = (PickingMode)(m_rootVisualElement.Q<Button>("cultivisionButton").pickingMode == 0 ? 1 : 0);
            m_rootVisualElement.Q<Button>("collectionsButton").pickingMode = (PickingMode)(m_rootVisualElement.Q<Button>("collectionsButton").pickingMode == 0 ? 1 : 0);
        }


        public void EnableButton(string button)
        {
            m_rootVisualElement.Q<Button>(button).visible = true;
            m_rootVisualElement.Q<Button>(button).pickingMode = PickingMode.Position;
        }

        public void DisableButton(string button)
        {
            m_rootVisualElement.Q<Button>(button).visible = false;
            m_rootVisualElement.Q<Button>(button).pickingMode = PickingMode.Ignore;
        }

        public void OpenHUD()
        {
            m_controller.OpenHUD();
        }

        public void CloseHUD()
        {
            m_controller.CloseHUD();
        }
        public void OpenHUDAnimated()
        {
            m_controller.OpenHUDAnimated();
        }

        public void CloseHUDAnimated()
        {
            m_controller.CloseHUDAnimated();
        }

        private void UpdateMoneyDisplay()
        {
            int currentMoney = EventManager.instance.HandleEVENT_INVENTORY_GET_MONEY();
            m_rootVisualElement.Q<Label>("currency-text").text = currentMoney.ToString();
        }
    }
}