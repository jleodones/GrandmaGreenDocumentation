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
        public Genotype genotype;
        public int growthStage;
        public float timePlanted;
        public Vector3Int cell;

        // waterStage acts like a bool (0 for unwatered, 1 for watered)
        // waterTimer keeps track of how long the plant has been "alive"
        public int waterStage;
        public int waterTimer;

        // State Manager for Fertilization Use
        public bool isFertilized;
    }

    [System.Serializable]
    public struct TileState
    {
        public Vector3Int cell;
        public int tileIndex;
    }

    [System.Serializable]
    public struct DecorState
    {
        public float x;
        public float y;

        public DecorationId ID;
    }

    [CreateAssetMenu(menuName = "GrandmaGreen/Garden/GardenManager")]
    public class GardenManager : ScriptableObject
    {

        [SerializeField]
        GardenSaver[] plantLookup;

        [SerializeField]
        public Timer[] timers;

        public int WiltTime;
        public int DeathTime;

        public Color dryTileTintColor = Color.white;
        public Color wateredTileTintColor;

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

        public void CreatePlant(PlantId type, Genotype genotype, int growthStage, int areaIndex, Vector3Int cell)
        {
            plantLookup[areaIndex][cell] = new PlantState
            {
                type = type,
                genotype = genotype,
                growthStage = growthStage,
                timePlanted = Time.time,
                cell = cell,

                // Adding for watering.
                waterStage = 0, // Adding a plant makes it unwatered
                waterTimer = 0, // Start timer at 0 and tick up
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

        public Genotype GetGenotype(int areaIndex, Vector3Int cell)
        {
            if (!IsEmpty(areaIndex, cell))
            {
                return plantLookup[areaIndex][cell].genotype;
            }
            return new Genotype();
        }

        public List<PlantState> GetNeighbors(int areaIndex, Vector3Int cell)
        {
            List<PlantState> neighbors = new List<PlantState>();
            foreach (Vector3Int neighbor in new[] {
                cell + Vector3Int.up,
                cell + Vector3Int.down,
                cell + Vector3Int.left,
                cell + Vector3Int.right})
            {
                neighbors.Add(GetPlant(areaIndex, neighbor));
            }
            return neighbors;
        }

        public List<PlantState> GetBreedingCandidates(int areaIndex, Vector3Int cell)
        {
            List<PlantState> candidates = new List<PlantState>();
            foreach (Vector3Int neighbor in new[] {
                cell + Vector3Int.up,
                cell + Vector3Int.down,
                cell + Vector3Int.left,
                cell + Vector3Int.right})
            {
                if (PlantIsBreedable(areaIndex, neighbor))
                {
                    candidates.Add(GetPlant(areaIndex, neighbor));
                }
            }
            return candidates;
        }

        public void UpdateGrowthStage(int areaIndex, Vector3Int cell)
        {
            PlantState plant = GetPlant(areaIndex, cell);
            int max = CollectionsSO.LoadedInstance.GetPlant(plant.type).growthStages;

            if (!IsEmpty(areaIndex, cell) && plant.growthStage < max - 1)
            {
                PlantState updatedPlant = plant;
                updatedPlant.growthStage = plant.growthStage + 1;

                //Reset the waterStage and waterTimers on growth
                updatedPlant.waterStage = 0;
                int supposedTimer = updatedPlant.waterTimer - CollectionsSO.LoadedInstance.GetPlant(plant.type).growthTime;
                //Debug.Log(supposedTimer);
                if (supposedTimer < 0)
                {
                    updatedPlant.waterTimer = 0;
                }
                else
                {
                    updatedPlant.waterTimer = supposedTimer;
                }

                plantLookup[areaIndex][cell] = updatedPlant;
            }
        }

        public void IncrementWaterTimer(int areaIndex, Vector3Int cell, int value)
        {
            if (!IsEmpty(areaIndex, cell))
            {
                PlantState plant = GetPlant(areaIndex, cell);
                plant.waterTimer += value;
                plantLookup[areaIndex][cell] = plant;
            }
        }

        public bool UpdateWaterStage(int areaIndex, Vector3Int cell)
        {
            bool waterStageUpdated = false;

            if (!IsEmpty(areaIndex, cell))
            {
                PlantState plant = GetPlant(areaIndex, cell);
                PlantState updatedPlant = plant;

                if (!PlantIsWilted(areaIndex, cell))
                {
                    if (PlantIsFullyGrown(areaIndex, cell))
                    {
                        updatedPlant.waterTimer = 0;
                    }
                    else
                    {
                        updatedPlant.waterStage = 1;

                        if (updatedPlant.waterTimer >= CollectionsSO.LoadedInstance.GetPlant(plant.type).growthTime)
                        {
                            waterStageUpdated = true;
                        }
                    }
                }
                else
                {
                    updatedPlant.waterTimer = 0;
                }

                plantLookup[areaIndex][cell] = updatedPlant;

            }

            return waterStageUpdated;
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

        public bool PlantIsFullyGrown(int areaIndex, Vector3Int cell)
        {
            PlantState plant = GetPlant(areaIndex, cell);
            PlantProperties properties = CollectionsSO.LoadedInstance.GetPlant(plant.type);
            return plant.growthStage == properties.growthStages - 1;
        }

        public bool PlantNeedsWater(int areaIndex, Vector3Int cell)
        {
            if (IsEmpty(areaIndex, cell))
                return false;


            PlantState plant = GetPlant(areaIndex, cell);
            PlantProperties properties = CollectionsSO.LoadedInstance.GetPlant(plant.type);

            if (PlantIsFullyGrown(areaIndex, cell))
                return plant.waterTimer > 0;
            else
                return plant.waterStage == 0;

        }

        public bool PlantIsWilted(int areaIndex, Vector3Int cell)
        {
            if (!IsEmpty(areaIndex, cell))
            {
                PlantState plant = GetPlant(areaIndex, cell);

                if (!PlantIsFullyGrown(areaIndex, cell))
                {
                    return plant.waterTimer >= WiltTime //240
                        && plant.waterTimer < DeathTime; //336;
                }
                else
                {
                    return plant.waterTimer >= WiltTime;
                }

            }
            else
                return false;
        }

        public bool PlantIsDead(int areaIndex, Vector3Int cell)
        {
            if (!IsEmpty(areaIndex, cell))
            {
                PlantState plant = GetPlant(areaIndex, cell);

                return plant.waterTimer >= DeathTime
                    && !PlantIsFullyGrown(areaIndex, cell); //336;

            }
            else
                return false;

        }

        public bool PlantIsBreedable(int areaIndex, Vector3Int cell)
        {
            if (!IsEmpty(areaIndex, cell) && PlantIsFullyGrown(areaIndex, cell))
            {
                return !PlantIsWilted(areaIndex, cell);
            }
            else
                return false;

        }

        public List<TileState> GetTiles(int areaIndex)
        {
            return new List<TileState>(plantLookup[areaIndex].Tiles());
        }

        public void UpdateGardenTile(int areaIndex, Vector3Int cell, int tileIndex)
        {
            plantLookup[areaIndex].SetTileState(new TileState()
            {
                cell = cell,
                tileIndex = tileIndex
            });
        }

        public void UpdateDecorItem(int areaIndex, DecorationId decorID, Vector3 newPosition, Vector3? oldPosition = null)
        {
            if (oldPosition != null)
            {
                plantLookup[areaIndex].RemoveDecorState(
                new DecorState()
                {
                    ID = decorID,
                    x = ((Vector3)oldPosition).x,
                    y = ((Vector3)oldPosition).y,
                }
                );
            }

            plantLookup[areaIndex].AddDecorState(
                new DecorState()
                {
                    ID = decorID,
                    x = newPosition.x,
                    y = newPosition.y,
                }
            );
        }

        public List<DecorState> GetDecor(int areaIndex)
        {
            return new List<DecorState>(plantLookup[areaIndex].Decor());
        }
    }
}
