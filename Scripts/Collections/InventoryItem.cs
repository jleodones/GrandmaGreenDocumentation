using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* InventoryItem is for each item currently in the player's inventory.
* Only care about item id and quantity of each item.
*/
namespace GrandmaGreen.Collections {
    [Serializable]
    public struct InventoryItem
    {
        public string id;
        public int quantity;
    }

}