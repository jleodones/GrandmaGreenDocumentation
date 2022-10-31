using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.Dialogue;
using GrandmaGreen.Entities;

namespace GrandmaGreen.UI.Golems
{
    public class GolemMenu : MonoBehaviour
    {
        /// <summary>
        /// Golem menu UI.
        /// </summary>
        public UIDocument golemMenu;

        /// <summary>
        /// Camera zoom for golems on touch.
        /// </summary>
        public CameraZoom cameraZoom;

        private bool m_isGolemActive = false;
        
        private bool m_isMenuOpen = false;
        void OnEnable()
        {
            // Register dialogue button callback.
            golemMenu.rootVisualElement.Q<Button>("dialogue").RegisterCallback<ClickEvent>(OnDialogueTrigger);
        }

        public void HandleGolemInteract()
        {
            m_isMenuOpen = !m_isMenuOpen;

            if (m_isMenuOpen)
            {
                GolemMenuOpen();
            }
            else
            {
                GolemMenuClose();
            }
        }
        
        public void GolemMenuOpen()
        {
            // Close the HUD.
            EventManager.instance.HandleEVENT_CLOSE_HUD();
            
            // Set menu location.
            SetLocation(GetComponentInParent<GolemController>().transform.position);
            GetComponentInParent<GolemController>().onEntityMove += SetLocation;
            
            // Display.
            golemMenu.rootVisualElement.style.display = DisplayStyle.Flex;
        }

        public void GolemMenuClose()
        {
            EventManager.instance.HandleEVENT_OPEN_HUD();
            golemMenu.rootVisualElement.style.display = DisplayStyle.None;
        }

        private void OnDialogueTrigger(ClickEvent clickEvent)
        {
            // Disable menu.
            golemMenu.rootVisualElement.style.display = DisplayStyle.None;

            // Find the golem's dialogue script.
            Dialogueable dialogueScript = GetComponentInChildren<Dialogueable>();
            
            // TODO: Check if grandma is within a certain distance. If so, play dialogue. If not, ignore.
            // Call the dialogue trigger.
            dialogueScript.TriggerDialogue();
        }
        
        public void SetLocation(Vector3 worldPosition)
        {
            golemMenu.rootVisualElement.transform.position =
                RuntimePanelUtils.CameraTransformWorldToPanel(golemMenu.rootVisualElement.panel, worldPosition, Camera.main);
        }
    }
}
