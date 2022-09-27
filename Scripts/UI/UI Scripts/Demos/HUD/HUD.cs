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
        
        public static HUD instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }

            UIDocument HUD = GetComponent<UIDocument>();
            VisualElement root = HUD.rootVisualElement;

            m_controller = new(root);

            m_controller.RegisterButtonCallbacks();
        }

        public void OpenHUD()
        {
            m_controller.OpenHUD();
        }
        
    }
}