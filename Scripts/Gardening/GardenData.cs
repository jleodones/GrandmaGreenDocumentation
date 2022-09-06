using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace GrandmaGreen.Garden
{
    [System.Serializable]
    /// <summary>
    /// Struct for plant state storage
    /// </summary>
    public struct PlantState
    {
        public int gridIndex;
        public int phenotypeIndex;
        public Genotype genotype;
    }

    [CreateAssetMenu(menuName = "GrandmaGreen/Garden/Garden Data")]
    /// <summary>
    /// SO for all data related to garden plants
    /// TODO: Support for garden decor
    /// </summary>
    public class GardenData : ScriptableObject
    {
        public Vector2Int gridSize;
        public Vector2 cellSize;
        public Vector3 worldOrigin;
        public List<PlantState> plantStates;

        public Vector3 IndexToWorldPos(int index)
        {
            Vector3 worldPos = Vector3.zero;

            worldPos.x = worldOrigin.x + index % gridSize.x * cellSize.x;
            worldPos.y = worldOrigin.y + index / gridSize.x * cellSize.y;

            return worldPos;
        }

        public Vector2Int WorldToGrid(Vector3 position)
        {
            Vector2Int gridPos = Vector2Int.zero;
            gridPos.x = (int)((position.x - worldOrigin.x) / cellSize.x);
            gridPos.y = (int)((position.y - worldOrigin.y) / cellSize.y);

            return gridPos;
        }
    }
}