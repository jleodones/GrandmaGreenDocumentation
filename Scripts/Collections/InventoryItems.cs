using System;
using System.Collections;
using System.Collections.Generic;
using GrandmaGreen.Garden;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GrandmaGreen.Collections {
    
    // Used as a wrapper for all the inventory item types.
    public interface IInventoryItem
    {
        public int itemID { get; set; }
        public ItemType itemType { get; set; }
        
        public string itemName { get; set; }

        public string GetQuantityToString();

    }
    public enum ItemType
    {
        Tool,
        Plant,
        Seed,
        Decor
    }

    [Serializable]
    public struct Tool : IInventoryItem
    {
        // Item ID.
        public int itemID { get; set; }

        // This item type;
        public ItemType itemType { get; set; }

        // Name of object.
        public string itemName { get; set; }
        
        // Amount of the object present in the inventory.
        public int quantity;

        public Tool(int id, string name, int num)
        {
            itemType = ItemType.Tool;
            
            itemID = id;
            itemName = name;
            quantity = num;
        }

        public string GetQuantityToString()
        {
            return quantity.ToString();
        }

        public override bool Equals(object obj) =>
            obj is IInventoryItem other && other != null && other.itemID == itemID && other.itemType == itemType;
    }
    
    [Serializable]
    public struct Plant : IInventoryItem
    {
        // Item ID.
        public int itemID { get; set; }
        
        // This item type;
        public ItemType itemType { get; set; }

        // Name of object.
        public string itemName { get; set; }
        
        // Amount of the object present in the inventory.
        public int quantity;

        public Trait trait;

        public Plant(int id, string name, int num, Trait plantTrait)
        {
            itemType = ItemType.Plant;
            
            itemID = id;
            itemName = name;
            quantity = num;
            trait = plantTrait;
        }
        
        public string GetQuantityToString()
        {
            return quantity.ToString();
        }
        public override bool Equals(object obj) =>
            obj is IInventoryItem other && other != null && other.itemID == itemID && other.itemType == itemType;
    }

    [Serializable]
    public struct Seed : IInventoryItem
    {
        // Item ID.
        public int itemID { get; set; }
        
        // This item type;
        public ItemType itemType { get; set; }

        // Name of object.
        public string itemName { get; set; }
        
        // Amount of the object present in the inventory.
        public int quantity;
        
        public Seed(int id, string name, int num)
        {
            itemType = ItemType.Seed;
            
            itemID = id;
            itemName = name;
            quantity = num;
        }
        
        public string GetQuantityToString()
        {
            return quantity.ToString();
        }
        
        public override bool Equals(object obj) =>
            obj is IInventoryItem other && other != null && other.itemID == itemID && other.itemType == itemType;
    }

    [Serializable]
    public struct Decor : IInventoryItem
    {
        // Item ID.
        public int itemID { get; set; }
        
        // This item type;
        public ItemType itemType { get; set; }

        // Name of object.
        public string itemName { get; set; }
        
        // Amount of the object present in the inventory.
        public int quantity;
        
        public Decor(int id, string name, int num)
        {
            itemType = ItemType.Decor;
            
            itemID = id;
            itemName = name;
            quantity = num;
        }
        
        public string GetQuantityToString()
        {
            return quantity.ToString();
        }
        
        public override bool Equals(object obj) =>
            obj is IInventoryItem other && other != null && other.itemID == itemID && other.itemType == itemType;
    }
}