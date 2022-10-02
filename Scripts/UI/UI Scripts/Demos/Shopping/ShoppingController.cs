using UnityEngine;
using System;
using System.Collections.Generic;
using GrandmaGreen;
using UnityEngine.UIElements;
using GrandmaGreen.SaveSystem;
using GrandmaGreen.Collections;
using Sirenix.OdinInspector;
using GrandmaGreen.UI.HUD;
using GrandmaGreen.Garden;

    namespace GrandmaGreen.UI.Collections
{
    public class ShoppingController
    {
        /* Member variables */
        private const string exitButton = "exit-button";
        private const string increaseButton = "increase-button";
        private const string decreaseButton = "decrease-button";
        private const string itemButton = "item-button";
        private const string buyButton = "buy-button";

        private const string popupElement = "popup-element";
        private const string itemNameElement = "name-element";
        private const string amountElement = "amount-element";
        private const string costElement = "cost-element";

        // The root of the shopping UI.
        private readonly VisualElement root;

        // the item that is chosen to buy??
        private ShoppingItem item;

        public ShoppingController(VisualElement _root)
        {
            // Set member variables.
            root = _root;

            // PROBS NEED TO SET INVENTORY STUFF TOO SO THINGS ACTUALLY GO INTO THE INVENTORY???
        }

        // WHEN POPUP NOT ACTIVE YET
        // Register the activate popup function to the event: when an item is clicked
        public void RegisterPopUpCallback()
        {
            Button b = root.Q<Button>(itemButton);
            b.RegisterCallback<ClickEvent>(ActivatePopup);
        }

        public void ActivatePopup(ClickEvent evt)
        {
            root.Q(popupElement).style.display = DisplayStyle.Flex;
            root.Q<Label>(itemNameElement).text = item.label;
            root.Q<Label>(costElement).text = item.cost.ToString();
            // would also change the item icon here??
        }

        // WHEN POPUP IS ACTIVE
        // Register the buy function to the event: when buy button is clicked
        public void RegisterBuyCallback()
        {
            root.Q<Button>(buyButton).RegisterCallback<ClickEvent>(BuyItem);
        }

        // Register the increase amt function to the event: when increase button is clicked???
        public void RegisterIncreaseAmountCallback()
        {
            root.Q<Button>(increaseButton).RegisterCallback<ClickEvent>(IncreaseAmt);
        }

        // Register the decrease amt function to the event: when decrease button is clicked???
        public void RegisterDecreaseAmountCallback()
        {
            root.Q<Button>(decreaseButton).RegisterCallback<ClickEvent>(DecreaseAmt);
        }

        // This "buys" the specified amount of the item and adds it to the inventory!
        private void BuyItem(ClickEvent evt)
        {
            // ADDS ITEM TO THE INVENTORY AND PROBS PLAYS A LIL ANIMATION IDK

            // Set the popup display to none.
            root.Q(popupElement).style.display = DisplayStyle.None;
        }

        // This increases the amount of the item, highest is 99 for now???
        private void IncreaseAmt(ClickEvent evt)
        {
            int amt = Int32.Parse(root.Q<Label>(amountElement).text);
            if (amt < 100)
            {
                amt++;
                root.Q<Label>(amountElement).text = "" + amt;
                root.Q<Label>(costElement).text = "" + (amt * item.cost);
            }
        }

        // This descreases the amount of the item, lowest is 1
        private void DecreaseAmt(ClickEvent evt)
        {
            int amt = Int32.Parse(root.Q<Label>(amountElement).text);
            if (amt > 0)
            {
                amt--;
                root.Q<Label>(amountElement).text = "" + amt;
                root.Q<Label>(costElement).text = "" + (amt * item.cost);
            }
        }
    }
}
