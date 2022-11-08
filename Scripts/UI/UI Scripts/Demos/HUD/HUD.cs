// This script attaches the tabbed menu logic to the game.

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

        private void Awake()
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