// This script attaches the tabbed menu logic to the game.

using System;
using GrandmaGreen.UI.Collections;
using UnityEngine;
using UnityEngine.UIElements;

//Inherits from class `MonoBehaviour`. This makes it attachable to a game object as a component.
namespace GrandmaGreen.UI.HUD
{
    public class HUD : MonoBehaviour
    {
        public TabbedInventory inventory;
        private HUDController m_controller;

        private void OnEnable()
        {
            UIDocument HUD = GetComponent<UIDocument>();
            VisualElement root = HUD.rootVisualElement;

            m_controller = new(root);

            m_controller.RegisterButtonCallbacks();
            
            // Register global events.
            EventManager.instance.EVENT_OPEN_HUD += OpenHUD;
            EventManager.instance.EVENT_OPEN_HUD_ANIMATED += OpenHUDAnimated;
            EventManager.instance.EVENT_CLOSE_HUD += CloseHUD;
            EventManager.instance.EVENT_CLOSE_HUD_ANIMATED += CloseHUDAnimated;

            EventManager.instance.EVENT_UPDATE_MONEY_DISPLAY += () =>
            {
                int currentMoney = EventManager.instance.HandleEVENT_INVENTORY_GET_MONEY();
                root.Q<Label>("currency-text").text = currentMoney.ToString();
            };

            root.Q<Button>("customization-button").clicked+=()=>
            {
                EventManager.instance.HandleEVENT_TOGGLE_CUSTOMIZATION_MODE();
            };
        }

        private void FixedUpdate()
        {
            // Set money count, on awake.
            EventManager.instance.HandleEVENT_UPDATE_MONEY_DISPLAY();
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
        
    }
}