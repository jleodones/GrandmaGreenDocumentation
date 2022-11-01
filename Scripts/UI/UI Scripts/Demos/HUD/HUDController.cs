// This script defines the tab selection logic.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen.UI.HUD
{
    public class HUDController
    {
        /*
        To-Do:
        - exit buttons: hide the current element, 
            show HUD element
        - buttons -> open element with same name
        */

        /* Define member variables*/

        private VisualElement m_root;
        private VisualElement m_draggable;

        public HUDController(VisualElement root)
        {
            m_root = root;
            Debug.Log(Screen.width);
            Debug.Log(Screen.height);
        }

        public void OpenHUD()
        {
            m_root.style.display = DisplayStyle.Flex;
        }
        public void OpenHUDAnimated()
        {
            m_root.style.display = DisplayStyle.Flex;
        }
         public void CloseHUD()
        {
            m_root.style.display = DisplayStyle.None;
        }

        public void RegisterButtonCallbacks()
        {
            // For now, this just registers the inventory callback.
            // TODO: Update this for every functional UI.
            Button inventoryButton = m_root.Q<Button>("inventory-button");
            inventoryButton.RegisterCallback<ClickEvent>(InventoryOnClick);
            UQueryBuilder<Button> buttons = m_root.Query<Button>();
            buttons.ForEach((Button hud_button) => { hud_button.RegisterCallback<ClickEvent>(ButtonOnClick); });

            // Adding draggable demo code here for now
            m_draggable = m_root.Q<VisualElement>("draggable");
            m_draggable.RegisterCallback<PointerDownEvent>(PointerDownHandler);
            m_draggable.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
            m_draggable.RegisterCallback<PointerUpEvent>(PointerUpHandler);
        }

        private Vector2 targetStartPosition { get; set; }
        private Vector3 pointerStartPosition { get; set; }

        private bool enabled { get; set; }

        private void PointerDownHandler(PointerDownEvent evt)
        {
            targetStartPosition = m_draggable.transform.position;
            pointerStartPosition = evt.position;
            m_draggable.CapturePointer(evt.pointerId);
            enabled = true;
        }

        private void PointerMoveHandler(PointerMoveEvent evt)
        {
            if (enabled && m_draggable.HasPointerCapture(evt.pointerId))
            {
                Vector3 pointerDelta = evt.position - pointerStartPosition;
                Debug.Log(targetStartPosition.y + pointerDelta.y);
                Debug.Log(targetStartPosition.x + pointerDelta.x);
                m_draggable.transform.position = new Vector2(
                    Mathf.Clamp(targetStartPosition.x + pointerDelta.x, 0, Screen.width - 150),
                    Mathf.Clamp(targetStartPosition.y + pointerDelta.y, 0, Screen.height-50));
            }
        }

        private void PointerUpHandler(PointerUpEvent evt)
        {
            if (enabled && m_draggable.HasPointerCapture(evt.pointerId))
            {
                m_draggable.ReleasePointer(evt.pointerId);
            }
        }

        public void InventoryOnClick(ClickEvent evt)
        {
            // Enables inventory.
            EventManager.instance.HandleEVENT_INVENTORY_OPEN();
        }

        public void ButtonOnClick(ClickEvent evt)
        {
            Button clickedButton = evt.currentTarget as Button;
            clickedButton.RegisterCallback<TransitionEndEvent>(OnScaleUp);
            clickedButton.style.transitionProperty = new List<StylePropertyName> { "scale"};
            clickedButton.style.transitionTimingFunction = new List<EasingFunction> { EasingMode.EaseIn };
            clickedButton.style.transitionDuration = new List<TimeValue>{ new TimeValue(300, TimeUnit.Millisecond)};
            clickedButton.style.transitionDelay = new List<TimeValue>{ new TimeValue(0, TimeUnit.Millisecond)};
            clickedButton.style.scale = new Scale(new Vector2(1.2f,1.2f));
        }

        public void OnScaleUp(TransitionEndEvent evt)
        {
            Button clickedButton = evt.currentTarget as Button;
            clickedButton.RegisterCallback<TransitionEndEvent>(OnScaleDown);
            clickedButton.UnregisterCallback<TransitionEndEvent>(OnScaleUp);
            clickedButton.style.scale = new Scale(new Vector2(0.9f, 0.9f));

        }
        public void OnScaleDown(TransitionEndEvent evt)
        {
            Button clickedButton = evt.currentTarget as Button;
            clickedButton.UnregisterCallback<TransitionEndEvent>(OnScaleDown);
            clickedButton.style.transitionDuration = new List<TimeValue>{ new TimeValue(400, TimeUnit.Millisecond)};
            clickedButton.style.scale = new Scale(new Vector2(1, 1));

        }
    }
}