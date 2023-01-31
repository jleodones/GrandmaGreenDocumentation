using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.Dialogue;
using GrandmaGreen.Entities;
using GrandmaGreen.UI.Collections;
using GrandmaGreen.UI.Shopping;

namespace GrandmaGreen
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

        public Dialogueable dialogueScript;



        public override void Load()
        {
            dialogueScript = GetComponentInChildren<Dialogueable>();

            // Register button callback.
            RegisterButtonCallbackWithClose("dialogue", OnDialogueTrigger);
            RegisterButtonCallbackWithClose("sell", OnSellTrigger);
            RegisterButtonCallbackWithClose("shop", OnShopTrigger);
        }

        public void HandleTap()
        {
            if (!m_isMenuOpen)
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
            EventManager.instance.HandleEVENT_CLOSE_HUD();

            // Get Collider bounding box
            Bounds box = GetComponentInParent<Collider>().bounds;

            // Set menu location based box size
            SetLocation(box.center);
            base.OpenUI();
        }

        public override void CloseUI()
        {
            m_isMenuOpen = false;

            EventManager.instance.HandleEVENT_OPEN_HUD();
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
            m_isMenuOpen = false;
            base.CloseUI();

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
            CloseUI();

            sellingUI.OpenUI();
        }

        public void OnShopTrigger()
        {
            // Close this UI first.
            CloseUI();

            // Then open shop UI.
            shopUI.OpenUI();
        }

        public void SetLocation(Vector3 worldPosition)
        {
            if (m_isMenuOpen)
            {
                Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
                    m_rootVisualElement.panel, worldPosition, Camera.main);
                m_rootVisualElement.transform.position = newPosition.WithNewX(newPosition.x -
                    m_rootVisualElement.layout.width / 2);
            }
        }
    }
}
