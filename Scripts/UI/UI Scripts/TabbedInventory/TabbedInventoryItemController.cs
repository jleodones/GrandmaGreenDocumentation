using System;
using GrandmaGreen.Collections;
using GrandmaGreen.Garden;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen.UI.Collections
{
    public class TabbedInventoryItemController : IDraggable
    {
        public IInventoryItem inventoryItemData;
        public Sprite sprite;
        
        // This button.
        public Button button { get; set; }

        // Draggable item's starting position.
        public Vector3 startingPosition { get; set; }

        public TabbedInventoryItemController(Button _button)
        {
            button = _button;
        }

        public void SetButtonCallback(System.Action<TabbedInventoryItemController> buttonClickCallback)
        {
            button.clicked += () =>
            {
                buttonClickCallback?.Invoke(this);
            };
        }

        //This function receives the character whose name this list 
        //element is supposed to display. Since the elements list 
        //in a `ListView` are pooled and reused, it's necessary to 
        //have a `Set` function to change which character's data to display.
        public void SetInventoryData(IInventoryItem inventoryItem, Sprite _sprite)
        {
            sprite = _sprite;
            
            button.Q<VisualElement>("item-image").style.backgroundImage = new StyleBackground(sprite);
            button.Q<Label>("quantity").text = inventoryItem.GetQuantityToString();
            button.Q<Label>("item-name").text = inventoryItem.itemName;
            
            inventoryItemData = inventoryItem;
        }

        public void SetSizeBadge(Genotype genotype)
        {
            Sprite sprite;
            // Set size badge.
            String str = "UI_Inventory_Badge_";
            switch (genotype.size)
            {
                case Genotype.Size.VerySmall:
                    str += "XS";
                    break;
                case Genotype.Size.Small:
                    str += "S";
                    break;
                case Genotype.Size.Medium:
                    str += "M";
                    break;
                case Genotype.Size.Big:
                    str += "L";
                    break;
                case Genotype.Size.VeryBig:
                    str += "XL";
                    break;
            }

            sprite = Resources.Load<Sprite>(str);
            
            button.Q<VisualElement>("size").style.backgroundImage = new StyleBackground(sprite);
            button.Q<VisualElement>("size").style.display = DisplayStyle.Flex;
        }

        public void SetAlpha(float a)
        {
            button.Q<VisualElement>("item-image").style.unityBackgroundImageTintColor = new Color(1, 1, 1, a);
            button.Q<Label>("quantity").style.color = new Color(0.45882f, 0.26275f, 0.14510f, a);
        }
    }
}
