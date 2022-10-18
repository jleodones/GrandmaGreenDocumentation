using System.Collections;
using System.Collections.Generic;
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
                Vector3 golemPosition = GetComponentInParent<Transform>().position;
                Debug.Log(golemPosition);
                golemMenu.rootVisualElement.transform.position =
                    RuntimePanelUtils.CameraTransformWorldToPanel(golemMenu.rootVisualElement.panel, golemPosition, Camera.main);
                
                // Makes menu visible and zooms in.
                golemMenu.rootVisualElement.Q("rootElement").style.display = DisplayStyle.Flex;
                cameraZoom.ZoomCameraRequest(4.2f, 0.5f);
            }
            else
            {
                // Makes menu invisible and zooms back out.
                golemMenu.rootVisualElement.Q("rootElement").style.display = DisplayStyle.None;
                cameraZoom.ZoomCameraRequest(5.0f, 0.5f);
            }
        }
    }
}
