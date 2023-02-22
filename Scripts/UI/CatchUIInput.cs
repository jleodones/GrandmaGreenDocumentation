using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using Core.Input;

namespace GrandmaGreen
{
    public class CatchUIInput : MonoBehaviour
    {
        public TouchInteraction touchInteraction;
        public UIDocument document;

        VisualElement root;

        public bool m_PointerActive = false;
        void OnEnable()
        {
            root = document.rootVisualElement;

            root.RegisterCallback<PointerUpEvent>(OnWorldPointerUp);
            root.RegisterCallback<PointerDownEvent>(OnWorldPointerDown);

            root.RegisterCallback<PointerMoveEvent>(OnWorldPointerMove);
            root.RegisterCallback<PointerOverEvent>(OnWorldPointerOver);

            root.RegisterCallback<PointerEnterEvent>(OnWorldPointerEnter);
            root.RegisterCallback<PointerOutEvent>(OnWorldPointerExit);

            root.RegisterCallback<PointerCaptureOutEvent>(OnWorldPointerOut);
            root.RegisterCallback<PointerLeaveEvent>(OnWorldPointerLeave);

        }

        void OnDisable()
        {
            root.UnregisterCallback<PointerUpEvent>(OnWorldPointerUp);
            root.UnregisterCallback<PointerDownEvent>(OnWorldPointerDown);

            root.UnregisterCallback<PointerMoveEvent>(OnWorldPointerMove);
            root.UnregisterCallback<PointerOverEvent>(OnWorldPointerOver);

            root.UnregisterCallback<PointerEnterEvent>(OnWorldPointerEnter);
            root.UnregisterCallback<PointerOutEvent>(OnWorldPointerExit);
            
            root.UnregisterCallback<PointerCaptureOutEvent>(OnWorldPointerOut);
            root.UnregisterCallback<PointerLeaveEvent>(OnWorldPointerLeave);


        }

        void OnWorldPointerDown(PointerDownEvent e)
        {
            m_PointerActive = true;
        }

        void OnWorldPointerUp(PointerUpEvent e)
        {
            m_PointerActive = false;
        }

        void OnWorldPointerMove(PointerMoveEvent e)
        {
            m_PointerActive = true;
        }

        void OnWorldPointerEnter(PointerEnterEvent e)
        {
            m_PointerActive = true;
        }

        void OnWorldPointerExit(PointerOutEvent e)
        {
            m_PointerActive = false;
        }

        void OnWorldPointerLeave(PointerLeaveEvent e)
        {
            m_PointerActive = false;
        }

        void OnWorldPointerOut(PointerCaptureOutEvent e)
        {
            m_PointerActive = false;
        }

        void OnWorldPointerOver(PointerOverEvent e)
        {
            m_PointerActive = true;
        }

        void Update()
        {
            if (m_PointerActive && touchInteraction.qPointerEvent != null)
            {
                touchInteraction.RaycastScreen();
            }



            touchInteraction.qPointerEvent = null;
            m_PointerActive = false;
        }
    }
}
