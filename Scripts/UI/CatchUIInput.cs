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

            root.RegisterCallback<PointerDownEvent>(OnWorldPointerDown);
            root.RegisterCallback<PointerMoveEvent>(OnWorldPointerMove);
            root.RegisterCallback<PointerUpEvent>(OnWorldPointerUp);
            root.RegisterCallback<PointerOutEvent>(OnWorldPointerExit);
            root.RegisterCallback<PointerEnterEvent>(OnWorldPointerEnter);
        }

        void OnDisable()
        {
            root.UnregisterCallback<PointerDownEvent>(OnWorldPointerDown);
            root.UnregisterCallback<PointerMoveEvent>(OnWorldPointerMove);
            root.UnregisterCallback<PointerUpEvent>(OnWorldPointerUp);
            root.UnregisterCallback<PointerOutEvent>(OnWorldPointerExit);
            root.UnregisterCallback<PointerEnterEvent>(OnWorldPointerEnter);
        }

        void OnWorldPointerDown(PointerDownEvent e)
        {
            m_PointerActive = true;
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

        void OnWorldPointerUp(PointerUpEvent e)
        {
            m_PointerActive = false;
        }

        void Update()
        {
            if (m_PointerActive && touchInteraction.qPointerEvent != null)
            {
                touchInteraction.RaycastScreen();
            }



            touchInteraction.qPointerEvent = null;
        }
    }
}
