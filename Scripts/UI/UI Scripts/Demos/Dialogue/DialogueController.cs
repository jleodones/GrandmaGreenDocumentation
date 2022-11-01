using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandmaGreen;

namespace GrandmaGreen
{
    public class DialogueController : MonoBehaviour
    {
        public Transform TransformToFollow;
        private VisualElement m_Bar;
        private Camera m_MainCamera;

        // Start is called before the first frame update
        void Start() 
        {
            m_MainCamera = Camera.main;
            m_Bar = GetComponent<UIDocument>().rootVisualElement.Q("container");
            SetPosition();
        }

        private void LateUpdate()
        {
            if (TransformToFollow != null)
            {
                SetPosition();
            }
        }
        public void SetPosition()
        {
            Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(
                m_Bar.panel, TransformToFollow.position, m_MainCamera);
            m_Bar.transform.position = newPosition.WithNewX(newPosition.x - 
                m_Bar.layout.width / 2);
        }
    }
}