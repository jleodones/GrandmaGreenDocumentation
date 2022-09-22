using UnityEngine;

namespace GrandmaGreen.Garden
{

    [System.Serializable]
    public struct Plant
    {
        public PlantType type;
        public int growthStage;
        public float timePlanted;
    }

    [CreateAssetMenu(menuName = "GrandmaGreen/Garden/GlobalPlantState")]
    public class PlantStateManager : ScriptableObject
    {
        [SerializeField]
        public Vector2Int[] gardenDimensions;
        public Plant[][,] plantData;

        public void Initialize()
        {
            gardenDimensions = new Vector2Int[5];
            plantData = new Plant[5][,];
        }

        public void RegisterGarden(Vector3Int dimensions, int areaIndex)
        {
            gardenDimensions[areaIndex] = new Vector2Int(dimensions.x, dimensions.y);
            plantData[areaIndex] = new Plant[dimensions.x, dimensions.y];
        }

        public void CreatePlant(PlantType type, int areaIndex, int x, int y)
        {
            plantData[areaIndex][x, y].type = type;
        }

        public void DestroyPlant(int areaIndex, int x, int y)
        {
            plantData[areaIndex][x, y] = new Plant();
        }

        public Plant GetPlant(int areaIndex, int x, int y)
        {
            return plantData[areaIndex][x, y];
        }

        public bool IsEmpty(int areaIndex, int x, int y)
        {
            return plantData[areaIndex][x, y].type == null;
        }

        [ContextMenu("DeleteGardenData")]
        public void DeleteGardenData()
        {
            plantData[0] = new Plant[gardenDimensions[0].x, gardenDimensions[0].y];
        }
    }
}
