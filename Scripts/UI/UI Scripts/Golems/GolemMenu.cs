using System.Collections;
using System.Collections.Generic;
using GrandmaGreen.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen.UI.Golems
{
    public class GolemMenu : MonoBehaviour
    {
        public UIDocument golemMenu;
        public CameraZoom cameraZoom;

        private bool m_isMenuShowing = false;

        public void TriggerMenu()
        {
            m_isMenuShowing = !m_isMenuShowing;
            
            if (m_isMenuShowing)
            {
                // Set menu location.
                // Vector3 golemPosition = GetComponentInParent<Transform>().position;
                SetLocation(GetComponentInParent<GolemController>().transform.position);
                GetComponentInParent<GolemController>().onEntityMove += SetLocation;

                // Makes menu visible and zooms in.
                golemMenu.rootVisualElement.Q("rootElement").style.display = DisplayStyle.Flex;
                cameraZoom.ZoomCameraRequest(4.2f, 0.5f);
            }
            else
            {
                // Makes menu invisible and zooms back out.
                GetComponentInParent<GolemController>().onEntityMove -= SetLocation;
                golemMenu.rootVisualElement.Q("rootElement").style.display = DisplayStyle.None;
                cameraZoom.ZoomCameraRequest(5.0f, 0.5f);
            }
        }

        public void SetLocation(Vector3 worldPosition)
        {
            golemMenu.rootVisualElement.transform.position =
                RuntimePanelUtils.CameraTransformWorldToPanel(golemMenu.rootVisualElement.panel, worldPosition, Camera.main);
        }
    }
}
