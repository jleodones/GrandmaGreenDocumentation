using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Collections {
    [Serializable]
    ///<summary>
    ///Temporary struct for an item in the Collections SO
    ///</summary>
    public struct Item
    {
        public int serialID;
        public string id;
        public string name;
        public string description;
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