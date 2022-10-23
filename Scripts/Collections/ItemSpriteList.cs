using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Collections {
    ///<summary>
    ///An item sprite contains the item's id and the sprite(s) associated with.
    ///</summary>
    [Serializable]
    public struct ItemSpriteList
    {
        public int id;
        public List<Sprite> sprites;
    }

}