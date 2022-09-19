using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Collections {
    [CreateAssetMenu(fileName = "New Item Sprites SO")]
    ///<summary>
    ///Item Sprite SO contains a List of ItemSprites of each item. Each ItemSprite has item name and sprite
    ///</summary>
    public class ItemSpritesSO : ScriptableObject
    {
        //dictionary won't show up in inspector
        //public Dictionary<string, Sprite> itemSprites = new Dictionary<string, Sprite>();
        public List<ItemSprite> itemSprites = new List<ItemSprite>();
    }
}
