using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Garden
{

    [System.Serializable]
    public struct Plant
    {
        public PlantType type;
        public int growthStage;
        public float timePlanted;
        public Vector3Int cell;
    }

    [CreateAssetMenu(menuName = "GrandmaGreen/Garden/GlobalPlantState")]
    public class PlantStateManager : ScriptableObject
    {
        [SerializeField]
        public Vector2Int[] gardenDimensions;
        public Dictionary<Vector3Int, Plant>[] plantLookup;
        [SerializeField]
        public List<Plant> plantListDebug;

        public void Initialize()
        {
            gardenDimensions = new Vector2Int[5];
            plantLookup = new Dictionary<Vector3Int, Plant>[5];
        }

        public void RegisterGarden(Vector3Int dimensions, int areaIndex)
        {
            gardenDimensions[areaIndex] = new Vector2Int(dimensions.x, dimensions.y);
            plantLookup[areaIndex] = new Dictionary<Vector3Int, Plant>();
        }

        public void CreatePlant(PlantType type, int areaIndex, Vector3Int cell)
        {
            plantLookup[areaIndex][cell] = new Plant
            {
                type = type,
                cell = cell
            };
        }

        public void DestroyPlant(int areaIndex, Vector3Int cell)
        {
            plantLookup[areaIndex].Remove(cell);
	    }

        public Plant GetPlant(int areaIndex, Vector3Int cell)
        {
            return plantLookup[areaIndex][cell];
        }

        public bool IsEmpty(int areaIndex, Vector3Int cell)
        {
            return !plantLookup[areaIndex].ContainsKey(cell);
        }

        [ContextMenu("DeleteGardenData")]
        public void DeleteGardenData()
        {
            plantLookup[0].Clear();
        }

        [ContextMenu("InspectPlants")]
        public void InspectPlants()
        {
            plantListDebug.Clear();
            foreach (Plant plant in plantLookup[0].Values)
            {
                plantListDebug.Add(plant); 
	        }
	    }
    }
}
