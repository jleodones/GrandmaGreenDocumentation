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
        public UnityEvent onWorldPointerDown;
        VisualElement root;

        void OnEnable()
        {
            root = document.rootVisualElement;
            root.RegisterCallback<PointerDownEvent>(OnWorldPointerDown);
        }

        void OnDisable()
        {
            root.UnregisterCallback<PointerDownEvent>(OnWorldPointerDown);
        }

        void OnWorldPointerDown(PointerDownEvent e)
        {
            Debug.Log("No UI Detected");
            onWorldPointerDown?.Invoke();
        }
    }
}
