using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Collections {
    [Serializable]
    public struct Item
    {
        public int serialID;
        public string id;
        public string name;
        public string description;
        //unlocked/locked depends on what is in the player's inventory
        public bool unlocked;
        //sprite to grab from assets:
        public Sprite image;
        //function to set the image variable - pull from assets folder when calling this
        //sprite naming convention: starts with id of item for now
        public void SetImage(Sprite img)
        {
            image = img;
        }
    }

}