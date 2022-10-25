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
            PlantState plant = GetPlant(areaIndex, cell);
            int max = collection.GetPlant(plant.type).growthStages;

            if (!IsEmpty(areaIndex, cell) && plant.growthStage < max)
            {
                plantLookup[areaIndex][cell] = new PlantState
                {
                    type = plant.type,
                    growthStage = plant.growthStage + 1,
                    timePlanted = plant.timePlanted,
                    cell = plant.cell
                };
            }
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
