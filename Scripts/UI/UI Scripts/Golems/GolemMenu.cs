using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using GrandmaGreen.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen.UI.Golems
{
    public class GolemMenu : MonoBehaviour
    {
        public UIDocument golemMenu;

        public UIDocument dialogue;

        private bool dialogueShowing = false;
        public CameraZoom cameraZoom;

        private bool m_isMenuShowing = false;

        void OnEnable()
        {
            // Register dialogue button callback.
            golemMenu.rootVisualElement.Q<Button>("dialogue").RegisterCallback<ClickEvent>(OnDialogueTap);
        }

        public void OnDialogueTap(ClickEvent evt)
        {
            dialogueShowing = !dialogueShowing;
            TriggerMenu();
        }

        public void TriggerMenu()
        {
            m_isMenuShowing = !m_isMenuShowing;
            
            if (m_isMenuShowing)
            {
                if (dialogueShowing)
                {
                    dialogueShowing = false;
                    dialogue.rootVisualElement.Q("rootElement").style.display = DisplayStyle.None;
                    GetComponentInParent<GolemController>().onEntityMove -= SetDialogueLocation;
                    m_isMenuShowing = false;
                    cameraZoom.ZoomCameraRequest(5.0f, 0.5f);
                }
                else
                {
                    // Set menu location.
                    // Vector3 golemPosition = GetComponentInParent<Transform>().position;
                    SetLocation(GetComponentInParent<GolemController>().transform.position);
                    GetComponentInParent<GolemController>().onEntityMove += SetLocation;
                
                    // Makes menu visible and zooms in.
                    golemMenu.rootVisualElement.Q("rootElement").style.display = DisplayStyle.Flex;
                    cameraZoom.ZoomCameraRequest(4.2f, 0.5f);
                }
            }
            else
            {
                // Makes menu invisible and zooms back out.
                GetComponentInParent<GolemController>().onEntityMove -= SetLocation;
                golemMenu.rootVisualElement.Q("rootElement").style.display = DisplayStyle.None;
                
                if (dialogueShowing)
                {
                    SetDialogueLocation(GetComponentInParent<GolemController>().transform.position);
                    GetComponentInParent<GolemController>().onEntityMove += SetDialogueLocation;
                    dialogue.rootVisualElement.Q("rootElement").style.display = DisplayStyle.Flex;
                }
                else
                {
                    cameraZoom.ZoomCameraRequest(5.0f, 0.5f);
                }
            }
        }

        public void SetDialogueLocation(Vector3 worldPosition)
        {
            dialogue.rootVisualElement.transform.position = RuntimePanelUtils.CameraTransformWorldToPanel(dialogue.rootVisualElement.panel, worldPosition, Camera.main);
        }

        public void SetLocation(Vector3 worldPosition)
        {
            golemMenu.rootVisualElement.transform.position =
                RuntimePanelUtils.CameraTransformWorldToPanel(golemMenu.rootVisualElement.panel, worldPosition, Camera.main);
        }
    }
}
