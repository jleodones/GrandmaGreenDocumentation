using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.Dialogue;
using GrandmaGreen.Entities;

namespace GrandmaGreen.UI.Golems
{
    public class GolemMenu : UIDisplayBase
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
        private GolemController golemController;

        void OnEnable()
        {
            // Register button callback.
            golemMenu.rootVisualElement.Q<Button>("dialogue").RegisterCallback<ClickEvent>(OnDialogueTrigger);
            golemMenu.rootVisualElement.Q<Button>("gift").RegisterCallback<ClickEvent>(OnGiftTrigger);
            golemMenu.rootVisualElement.Q<Button>("task").RegisterCallback<ClickEvent>(OnTaskTrigger);

            golemController =  GetComponentInParent<GolemController>();
        }

        public void ToggleMenu(bool isOpen)
        {
            if (m_isMenuOpen == isOpen) return;
            m_isMenuOpen = isOpen;

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
            // Get Collider bounding box
            Bounds box = golemController.GetGolemBox();

            // Set menu location based box size
            SetLocation(transform.position);
            GetComponentInParent<GolemController>().onEntityMove += SetLocation;
            
            // Display.
            m_rootVisualElement.style.display = DisplayStyle.Flex;
        }

        public void GolemMenuClose()
        {
            m_rootVisualElement.style.display = DisplayStyle.None;
        }

        private void OnDialogueTrigger(ClickEvent clickEvent)
        {
            // Disable menu.
            m_rootVisualElement.style.display = DisplayStyle.None;

            // Find the golem's dialogue script.
            Dialogueable dialogueScript = GetComponentInChildren<Dialogueable>();

            //zoom in the camera upon dialogue trigger
            if (golemController.golemManager.cameraZoom)
            {
                //need to switch the camera target to golem and back to grandma
                GameEntity gram = golemController.golemManager.playerController.entity;
                GameObject golemCameraTar = golemController.cameraTarget;
                GameObject gramCameraTar = gram.cameraTarget;
                dialogueScript.PauseAndZoomInEntity(4.0f, (CameraZoom)(golemController.golemManager.cameraZoom), golemCameraTar, gramCameraTar);
            }

            // Call the dialogue trigger.
            dialogueScript.TriggerDialogue();
        }

        private void OnGiftTrigger(ClickEvent clickEvent)
        {
            ushort gid = golemController.GetGolemID();

            EventManager.instance.HandleEVENT_GOLEM_HAPPINESS_UPDATE(gid, 30);
        }

        private void OnTaskTrigger(ClickEvent clickEvent)
        {
            ushort gid = golemController.GetGolemID();

            EventManager.instance.HandleEVENT_GOLEM_ASSIGN_TASK(gid);
        }
        
        public void SetLocation(Vector3 worldPosition)
        {
            if (m_isMenuOpen) {
                Bounds box = golemController.GetGolemBox();
                Vector3 UIpos = box.center + new Vector3(0, box.extents.y + 1.2f, 0);
                Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
                    m_rootVisualElement.panel, UIpos, Camera.main);

                m_rootVisualElement.transform.position = newPosition;
            }
        }
    }
}
