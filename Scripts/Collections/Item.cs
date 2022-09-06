using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Collections {
    [Serializable]
    public struct Item
    {
        public string id;
        public string name;
        public string description;
        //unlocked/locked depends on what is in the player's inventory
        public bool unlocked;
        //sprite to grab from assets:
    }

}