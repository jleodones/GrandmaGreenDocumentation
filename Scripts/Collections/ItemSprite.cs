using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Collections {
    [Serializable]
    ///<summary>
    ///An item sprite contains the item's name and the sprite associated with. Values inputted manually.
    ///</summary>
    public struct ItemSprite
    {
        public string name;
        public Sprite img;
    }

}