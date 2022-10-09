using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Collections {

    public enum itemTypes
    {
            Tool,
            Plant,
            Seed,
            Decor,
            Character
    }
    ///<summary>
    ///Temporary struct for an item in the Collections SO
    ///</summary>
    
    [Serializable]
    public struct Item
    {
        public int id;
        public itemTypes itemType;
        public string name;
        public string description;
        //only have aesthetic tag for decor items
        public string tag;
        //sprite to grab from assets:
        public Sprite image;
        //function to set the image variable - pull from assets folder when calling this
        //sprite naming convention: starts with id of item for now
        ///<summary>
        ///Set the image variable by pulling the asset sprite.
        ///Sprite naming convention: starts with name of item (spaces replaced with underscores) + _img
        ///</summary>
        public void SetImage(Sprite img)
        {
            image = img;
        }
    }

}