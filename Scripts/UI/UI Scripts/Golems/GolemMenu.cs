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
        private VisualElement root;
        public Transform TransformToFollow;


        void OnEnable()
        {
            // Register dialogue button callback.
            golemMenu.rootVisualElement.Q<Button>("dialogue").RegisterCallback<ClickEvent>(OnDialogueTrigger);
            root = golemMenu.rootVisualElement.Q("rootElement");
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
            SetLocation(TransformToFollow.position);
            GetComponentInParent<GolemController>().onEntityMove += SetLocation;
            
            // Display.
            root.style.display = DisplayStyle.Flex;
        }

        public void GolemMenuClose()
        {
            EventManager.instance.HandleEVENT_OPEN_HUD();
            root.style.display = DisplayStyle.None;
        }

        private void OnDialogueTrigger(ClickEvent clickEvent)
        {
            // Disable menu.
            root.style.display = DisplayStyle.None;

            // Find the golem's dialogue script.
            Dialogueable dialogueScript = GetComponentInChildren<Dialogueable>();
            
            // TODO: Check if grandma is within a certain distance. If so, play dialogue. If not, ignore.
            // Call the dialogue trigger.
            dialogueScript.TriggerDialogue();
        }
        
        public void SetLocation(Vector3 worldPosition)
        {
            Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
                root.panel, TransformToFollow.position, Camera.main);
            root.transform.position = newPosition.WithNewX(newPosition.x -
                root.layout.width / 2);
        }
    }
}
