using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GrandmaGreen
{
    [CreateAssetMenu(menuName = "GrandmaGreen/TileType")]
    public class TileType : ScriptableObject
    {
        public TileBase tile;

        public bool isPathable;
        public bool isPlottable;
        public bool isPlantable;
    }
}
