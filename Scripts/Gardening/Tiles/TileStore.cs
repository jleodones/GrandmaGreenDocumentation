using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct TileData
{
    public Tile tile;
    public bool pathable;
    public bool plantable;
    public bool plottable;
}

[CreateAssetMenu(fileName = "TileStore", menuName = "GrandmaGreen/TileStore", order = 0)]
public class TileStore : ScriptableObject
{
    public List<TileData> tileData;

    public TileData this[int i]
    {
        get { return tileData[i]; }
        set { tileData[i] = value; }
    }

}
