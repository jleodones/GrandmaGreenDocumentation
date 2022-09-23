using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Collections {
    [Serializable]
    public struct Tool
    {
        // Inventory item sprite.
        public Sprite itemSprite;
        
        // Name of object.
        public string itemName;
        
        // Amount of the object present in the inventory.
        public int quantity;
    }
    
    [Serializable]
    public struct Plant
    {
        // Inventory item sprite.
        public Sprite itemSprite;
        
        // Name of object.
        public string itemName;
        
        // Amount of the object present in the inventory.
        public int quantity;
    }

    [Serializable]
    public struct Seed
    {
        // Inventory item sprite.
        public Sprite itemSprite;
        
        // Name of object.
        public string itemName;
        
        // Amount of the object present in the inventory.
        public int quantity;
    }

    [Serializable]
    public struct Decor
    {
        // Inventory item sprite.
        public Sprite itemSprite;
        
        // Name of object.
        public string itemName;
        
        // Amount of the object present in the inventory.
        public int quantity;
        
        // Aesthetic tags.
        public string[] tags;
    }
}