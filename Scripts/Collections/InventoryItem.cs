using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///InventoryItem is for each item currently in the player's inventory.
///Only care about item id and quantity of each item.
///</summary>
namespace GrandmaGreen.Collections {
    [Serializable]
    public struct InventoryItem
    {
        public int serialID;
        public string ID;
        public int quantity;
    }

}