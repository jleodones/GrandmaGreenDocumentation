using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Collections {
    ///<summary>
    ///An item sprite contains the item's name and the sprite associated with. Values inputted manually.
    ///</summary>
    [Serializable]
    public struct ItemSprite
    {
        public int id;
        public Sprite img;
    }

}