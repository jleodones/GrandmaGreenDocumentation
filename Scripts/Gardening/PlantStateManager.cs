using System.Collections.Generic;
using GrandmaGreen.Collections;
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

    [System.Serializable]
    public struct GrowthEvent
    {
        public int growthTime;
        public int areaIndex;
        public Vector3Int cell;
    }

    [CreateAssetMenu(menuName = "GrandmaGreen/Garden/GlobalPlantState")]
    public class PlantStateManager : ScriptableObject
    {
        [SerializeField]
        CollectionsSO collection;

        private Dictionary<Vector3Int, PlantState>[] plantLookup;

        [SerializeField]
        List<GrowthEvent> growthEventQueue;

        [SerializeField]
        Timer timer;

        private void InsertGrowthEvent(GrowthEvent item)
        {
            if (growthEventQueue.Count == 0)
            {
                growthEventQueue.Add(item);
            }
            else if (item.growthTime > growthEventQueue[^1].growthTime)
            {
                growthEventQueue.Add(item);
            }
            else if (item.growthTime < growthEventQueue[0].growthTime)
            {
                growthEventQueue.Insert(0, item);
            }
            else
            {
                int index = growthEventQueue.BinarySearch(
                    item, Comparer<GrowthEvent>.Create((x, y)
                    => x.growthTime.CompareTo(y.growthTime)));
                index = index < 0 ? ~index : index;
                growthEventQueue.Insert(index, item);
            }
        }

        public void Initialize()
        {
            plantLookup = new Dictionary<Vector3Int, PlantState>[5];
            growthEventQueue = new List<GrowthEvent>();
        }

        public void RegisterGarden(int areaIndex)
        {
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
            return new List<PlantState>(plantLookup[areaIndex].Values);
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

        public bool UpdateGrowthStage(int areaIndex, Vector3Int cell)
        {
            PlantState plant = GetPlant(areaIndex, cell);
            int max = collection.GetPlant(plant.type).growthStages;

            bool canGrow = false;
            if (!IsEmpty(areaIndex, cell) && plant.growthStage < max-1)
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
                int waterRequirements= collection.GetPlant(plant.type).waterPerStage;

                if(plant.waterStage < waterRequirements) //&& plant.isWatered == false)
                {
                    PlantState updatedPlant = plant;
                    updatedPlant.waterStage = plant.waterStage + 1;
                    updatedPlant.waterTimer = 0;
                    updatedPlant.isWatered = true;

                    if(updatedPlant.waterStage == waterRequirements)
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

            if(!IsEmpty(areaIndex, cell))
            {
                PlantState plant = GetPlant(areaIndex, cell);

                if(!(plant.isFertilized))
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

            if(plant.isFertilized)
            {
                seedDrop = 2;
            }

            return seedDrop;
        }

        [ContextMenu("FireGrowthEvent")]
        public void FireGrowthEvent()
        {
            EventManager.instance.HandleEVENT_PLANT_UPDATE(0, Vector3Int.zero);
        }

        [ContextMenu("InsertGrowthEvent")]
        public void InsertGrowthEvent()
        {
            InsertGrowthEvent(new GrowthEvent { growthTime = (int)(10 * Random.value) });
        }
    }
}
