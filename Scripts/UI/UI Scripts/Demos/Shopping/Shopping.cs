using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.SaveSystem;
using GrandmaGreen.Collections;

namespace GrandmaGreen.UI.Collections
{
    public class Shopping : MonoBehaviour
    {
        private ShoppingController m_controller;
        public ShoppingItem item;

        void OnEnable()
        {
            // Gets the root of the tabbed inventory, which holds all the tabs in it.
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            // Sets up the controller for the whole inventory. The controller instantiates the inventory on its own upon creation.
            m_controller = new(root);

            // Register player events.
            m_controller.RegisterPopUpCallback();
            m_controller.RegisterBuyCallback();
            m_controller.RegisterIncreaseAmountCallback();
            m_controller.RegisterDecreaseAmountCallback();

            // create test shopping item
            item = new ShoppingItem();
        }
    }
}
