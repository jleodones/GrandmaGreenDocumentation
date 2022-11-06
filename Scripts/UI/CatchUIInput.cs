using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

namespace GrandmaGreen
{
    public class CatchUIInput : MonoBehaviour
    {
        public UIDocument document;
        public UnityEvent onWorldPointerEvent;
        VisualElement root;

        void OnEnable()
        {
            root = document.rootVisualElement;
            root.RegisterCallback<PointerDownEvent>(OnWorldPointerDown);
            root.RegisterCallback<PointerMoveEvent>(OnWorldPointerMove);
        }

        void OnDisable()
        {
            root.UnregisterCallback<PointerDownEvent>(OnWorldPointerDown);
            root.RegisterCallback<PointerMoveEvent>(OnWorldPointerMove);
        }

        void OnWorldPointerDown(PointerDownEvent e)
        {
            Debug.Log("No UI Detected");
            onWorldPointerEvent?.Invoke();
        }

        void OnWorldPointerMove(PointerMoveEvent e)
        {
            //Debug.Log("No UI Detected");
            onWorldPointerEvent?.Invoke();
        }
    }
}
