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

        public HUDController(VisualElement root)
        {
            m_root = root;
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