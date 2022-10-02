using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen
{
    public class ShoppingItem : MonoBehaviour
    {
        public int cost = 100;
        public string label = "rose seed";
        public Sprite icon;
        // i feel like maybe in the future we will have to add a "Resources" folder so that we can dynamically load sprites
        // but rn im too scared to add that so imma no do it hehe :D

        public ShoppingItem()
        {
            cost = 100;
            label = "rose seed";
        }
    }
}
