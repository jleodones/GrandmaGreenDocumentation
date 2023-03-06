using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.UIElements;

namespace GrandmaGreen
{


    public class TapHereUIHandler : MonoBehaviour
    {
        public Canvas tapHereCanvas;
        public PanelSettings panelSettings;
        public float pulseTime = 1.0f;

        public RectTransform tapHereUI;
        public Transform tapHereObject;

        Vector3 worldPos;


        bool updatePosition = false;
        public void SetTapHerePosition(Vector3 worldPosition)
        {
            tapHereObject.transform.position = worldPosition;
        }

        public void SetTapHerePosition(Vector2 screenPosition)
        {
            ((RectTransform)tapHereUI.transform).anchoredPosition = screenPosition;
        }

        VisualElement visualElementthing;
        public void SetTapHerePosition(VisualElement visualElement, VisualElement root)
        {
            //((RectTransform)tapHereUI.transform).anchoredPosition = Screen.height - visualElement.transform.position;
            visualElementthing = visualElement;
            visualElement.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            Debug.Log(visualElementthing.transform.position);
            
            Rect bounds = evt.newRect;
            Vector2 screenPos = bounds.center;

            ((RectTransform)tapHereUI.transform).anchoredPosition = visualElementthing.transform.position;

        }

    }
}
