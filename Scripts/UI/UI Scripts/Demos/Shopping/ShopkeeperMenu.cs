using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.Dialogue;
using GrandmaGreen.UI.Selling;

namespace GrandmaGreen.UI.Shopping
{
    public class ShopkeeperMenu : UIDisplayBase
    {
        /// <summary>
        /// Shopping UI.
        /// </summary>
        public ShoppingUI shopUI;

        public SellingUIDisplay sellingUI;

        /// <summary>
        /// Camera zoom on touch.
        /// </summary>
        public CameraZoom cameraZoom;
        private bool m_isMenuOpen = false;
        private bool m_isInteracting = false;

        public Dialogueable dialogueScript;

        private Bounds m_bounds;
        public GameObject player;

        private void Update()
        {
            if (m_isMenuOpen)
                SetLocation(transform.position);

            if (m_isInteracting)
            {
                // facing to grandma
                Vector3 playerPos = player.transform.position;

                if ((playerPos - transform.position).sqrMagnitude <= 5.0f)
                {
                    ToggleMenu(true);
                }
                else
                {
                    ToggleMenu(false);
                }
            }
        }

        public override void Load()
        {
            dialogueScript = GetComponentInChildren<Dialogueable>();
            m_bounds = GetComponentInParent<Collider>().bounds;

            // Register button callback.
            RegisterButtonCallbackWithClose("dialogue", OnDialogueTrigger);
            RegisterButtonCallbackWithClose("sell", OnSellTrigger);
            RegisterButtonCallbackWithClose("shop", OnShopTrigger);
        }

        public void HandleTap()
        {
            if (!m_isMenuOpen)
            {
                EventManager.instance.HandleEVENT_GOLEM_GRANDMA_MOVE_TO(transform.position);
                m_isInteracting = true;
            }
            else
            {
                ToggleMenu(false);
                m_isInteracting = false;
            }
        }

        public void ToggleMenu(bool isOpen)
        {
            if (m_isMenuOpen == isOpen) return;
            m_isMenuOpen = isOpen;

            if (m_isMenuOpen)
            {
                OpenUI();
            }
            else
            {
                CloseUI();
            }
        }

        public override void OpenUI()
        {
            m_isMenuOpen = true;

            // Close the HUD.
            // EventManager.instance.HandleEVENT_CLOSE_HUD();

            // Get Collider bounding box
            //Bounds box = GetComponentInParent<Collider>().bounds;

            // Set menu location based box size
            SetLocation(transform.position);
            base.OpenUI();
        }

        public override void CloseUI()
        {
            m_isMenuOpen = false;
            m_isInteracting = false;

            // EventManager.instance.HandleEVENT_OPEN_HUD();
            base.CloseUI();
        }

        string node;
        bool specialDialogue = false;
        public void SetSpecialDialogue(string node)
        {
            this.node = node;
            specialDialogue = true;
        }

        public void OnDialogueTrigger()
        {
            ToggleMenu(false);

            // EventManager.instance.HandleEVENT_CLOSE_HUD();

            // Call the dialogue trigger.

            if (specialDialogue)
            {
                dialogueScript.TriggerDialogueByNode(node);
                specialDialogue = false;
            }

            else
                dialogueScript.TriggerDialogue();
        }

        public void OnSellTrigger()
        {
            // Close menu.
            ToggleMenu(false);

            // EventManager.instance.HandleEVENT_CLOSE_HUD();

            sellingUI.OpenUI();
        }

        public void OnShopTrigger()
        {
            // Close this UI first.
            ToggleMenu(false);

            // EventManager.instance.HandleEVENT_CLOSE_HUD();

            // Then open shop UI.
            shopUI.OpenUI();
        }

        public void SetLocation(Vector3 worldPosition)
        {
            if (m_isMenuOpen)
            {
                Vector3 UIpos = m_bounds.center + new Vector3(0, m_bounds.extents.y + 0.8f, 0);
                Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
                    m_rootVisualElement.panel, UIpos, Camera.main);

                m_rootVisualElement.transform.position = newPosition;

                //Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
                //    m_rootVisualElement.panel, worldPosition, Camera.main);
                //m_rootVisualElement.transform.position = newPosition.WithNewX(newPosition.x -
                //    m_rootVisualElement.layout.width / 2);
            }
        }

        public void ReleaseShopkeeper()
        {
            if (!m_isInteracting) return;

            m_isInteracting = false;
            ToggleMenu(false);
        }
    }
}
