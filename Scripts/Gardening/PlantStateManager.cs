using System.Collections.Generic;

using UnityEngine;

namespace GrandmaGreen.Garden
{

    [System.Serializable]
    /// <summary>
    /// Struct for plant state storage
    /// </summary>
    public struct PlantState
    {
        public PlantType type;
        public int growthStage;
        public float timePlanted;
        public Vector3Int cell;

        public PlantState(PlantState plant)
        {
            this.type = plant.type;
            this.growthStage = plant.growthStage + 1 < type.growthStages ?
                plant.growthStage + 1 : plant.growthStage;
            this.timePlanted = plant.timePlanted;
            this.cell = plant.cell;
        }
    }

    [CreateAssetMenu(menuName = "GrandmaGreen/Garden/GlobalPlantState")]
    public class PlantStateManager : ScriptableObject
    {
        private Vector2Int[] gardenDimensions;
        private Dictionary<Vector3Int, PlantState>[] plantLookup;

        public void Initialize()
        {
            gardenDimensions = new Vector2Int[5];
            plantLookup = new Dictionary<Vector3Int, PlantState>[5];
        }

        public void RegisterGarden(Vector3Int dimensions, int areaIndex)
        {
            gardenDimensions[areaIndex] = new Vector2Int(dimensions.x, dimensions.y);
            plantLookup[areaIndex] = new Dictionary<Vector3Int, PlantState>();
        }

        public void ClearGarden(int areaIndex)
        {
            plantLookup[areaIndex] = new Dictionary<Vector3Int, PlantState>();
        }

        public bool IsEmpty(int areaIndex, Vector3Int cell)
        {
            return !plantLookup[areaIndex].ContainsKey(cell);
        }

        public void CreatePlant(PlantType type, int areaIndex, Vector3Int cell)
        {
            plantLookup[areaIndex][cell] = new PlantState
            {
                type = type,
                growthStage = 0,
                timePlanted = Time.time,
                cell = cell
            };
        }

        public void DestroyPlant(int areaIndex, Vector3Int cell)
        {
            if (!IsEmpty(areaIndex, cell))
            {
                plantLookup[areaIndex].Remove(cell);
            }
        }

        public PlantState GetPlant(int areaIndex, Vector3Int cell)
        {
            if (!IsEmpty(areaIndex, cell))
            {
                return plantLookup[areaIndex][cell];
            }
            return new PlantState();
        }

        public List<PlantState> GetPlants(int areaIndex)
        {
            return new List<PlantState>(plantLookup[areaIndex].Values);
        }

        public PlantType GetPlantType(int areaIndex, Vector3Int cell)
        {
            if (!IsEmpty(areaIndex, cell))
            {
                return plantLookup[areaIndex][cell].type;
            }
            return null;
        }

        public int GetGrowthStage(int areaIndex, Vector3Int cell)
        {
            if (!IsEmpty(areaIndex, cell))
            {
                return plantLookup[areaIndex][cell].growthStage;
            }
            return -1;
        }

        public void IncrementGrowthStage(int areaIndex, Vector3Int cell)
        {
            PlantState plant = plantLookup[areaIndex][cell];
            int current = plant.growthStage;
            int max = plant.type.growthStages;

            if (!IsEmpty(areaIndex, cell) && current < max)
            {
                plantLookup[areaIndex][cell] = new PlantState(plant);
            }
        }
    }
}
