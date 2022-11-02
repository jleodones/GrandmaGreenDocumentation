using GrandmaGreen.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Garden
{
    using Timer = TimeLayer.TimeLayer;

    [System.Serializable]
    public struct PlantState
    {
        public PlantId type;
        public int growthStage;
        public float timePlanted;
        public Vector3Int cell;

        // Counters and State Manager for Watering Tool Use
        public int waterStage;
        public int waterTimer;
        public bool isWatered;

        // State Manager for Fertilization Use
        public bool isFertilized;
    }

    [CreateAssetMenu(menuName = "GrandmaGreen/Garden/GardenManager")]
    public class GardenManager : ScriptableObject
    {
        [SerializeField]
        CollectionsSO collection;

        [SerializeField]
        GardenSaver[] plantLookup;

        [SerializeField]
        Timer timer;

        public void Initialize()
        {
        }

        public void RegisterGarden(int areaIndex)
        {
            plantLookup[areaIndex].Initialize();
        }

        public void ClearGarden(int areaIndex)
        {
            plantLookup[areaIndex].Clear();
        }

        public bool IsEmpty(int areaIndex, Vector3Int cell)
        {
            return !plantLookup[areaIndex].ContainsKey(cell);
        }

        public void CreatePlant(PlantId type, int areaIndex, Vector3Int cell)
        {
            plantLookup[areaIndex][cell] = new PlantState
            {
                type = type,
                growthStage = 0,
                timePlanted = Time.time,
                cell = cell,

                // Adding for watering. I think when we create plants it makes the most sense
                // to set the watering stage and timer here.
                waterStage = 0, // # of times it has been watered
                waterTimer = 0, // Could be deleted, but might be able to do it through Time
                isWatered = false,
                isFertilized = false
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
            return new List<PlantState>(plantLookup[areaIndex].Values());
        }

        public PlantId GetPlantType(int areaIndex, Vector3Int cell)
        {
            if (!IsEmpty(areaIndex, cell))
            {
                return plantLookup[areaIndex][cell].type;
            }
            return 0;
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
            if (!IsEmpty(areaIndex, cell))
            {
                PlantState plant = GetPlant(areaIndex, cell);
                PlantProperties p = collection.GetPlant(plant.type);
                plantLookup[areaIndex][cell] = new PlantState
                {
                    growthStage = plant.growthStage < p.growthStages - 1 ?
                        plant.growthStage + 1 : p.growthStages - 1,
                    type = plant.type,
                    timePlanted = plant.timePlanted,
                    cell = plant.cell
                };
            }
        }

        public bool UpdateGrowthStage(int areaIndex, Vector3Int cell)
        {
            PlantState plant = GetPlant(areaIndex, cell);
            int max = collection.GetPlant(plant.type).growthStages;

            bool canGrow = false;
            if (!IsEmpty(areaIndex, cell) && plant.growthStage < max - 1)
            {
                PlantState updatedPlant = plant;
                updatedPlant.growthStage = plant.growthStage + 1;
                updatedPlant.waterStage = 0;
                updatedPlant.waterTimer = 0;
                updatedPlant.isWatered = false;

                plantLookup[areaIndex][cell] = updatedPlant;
                canGrow = true;
            }

            return canGrow;
        }

        public bool UpdateWaterStage(int areaIndex, Vector3Int cell)
        {
            bool plantGrowth = false;

            if (!IsEmpty(areaIndex, cell))
            {
                PlantState plant = GetPlant(areaIndex, cell);
                int waterRequirements = collection.GetPlant(plant.type).waterPerStage;

                if (plant.waterStage < waterRequirements) //&& plant.isWatered == false)
                {
                    PlantState updatedPlant = plant;
                    updatedPlant.waterStage = plant.waterStage + 1;
                    updatedPlant.waterTimer = 0;
                    updatedPlant.isWatered = true;

                    if (updatedPlant.waterStage == waterRequirements)
                    {
                        plantGrowth = true;
                    }

                    plantLookup[areaIndex][cell] = updatedPlant;
                }
            }

            return plantGrowth;
        }

        public bool SetFertilization(int areaIndex, Vector3Int cell)
        {
            bool fertilizeSuccessful = false;

            if (!IsEmpty(areaIndex, cell))
            {
                PlantState plant = GetPlant(areaIndex, cell);

                if (!(plant.isFertilized))
                {
                    PlantState updatedPlant = plant;
                    updatedPlant.isFertilized = true;
                    plantLookup[areaIndex][cell] = updatedPlant;
                    fertilizeSuccessful = true;
                }
            }

            return fertilizeSuccessful;
        }

        public int NumSeedDrops(int areaIndex, Vector3Int cell)
        {
            PlantState plant = GetPlant(areaIndex, cell);
            int seedDrop = 1;

            if (plant.isFertilized)
            {
                seedDrop = 2;
            }

            return seedDrop;
        }
    }
}

