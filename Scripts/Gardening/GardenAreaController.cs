using GrandmaGreen.Collections;
using GrandmaGreen.Entities;
using GrandmaGreen.TimeLayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GrandmaGreen.Garden
{
    /// <summary>
    /// Responsible for functionality of a single Garden screen
    /// TODO: Implement functionality for save/loading changed tiles 
    /// </summary>
    public class GardenAreaController : AreaController
    {
        [Header("Collections")]
        [SerializeField]
        CollectionsSO collection;

        [Header("Player References")]
        public PlayerToolData playerTools;

        [Header("Garden Management")]
        public GardenManager gardenManager;
        public Dictionary<Vector3Int, GameObject> plantPrefabLookup;
        public List<PlantState> plantListDebug;
        public TileStore tileStore;

        [Header("Golem Management")]
        public GolemManager golemManager;

        [Header("Temporary Effects")]
        public ParticleSystem FertilizerParticleBurst;
        public ParticleSystem WateringParticleBurst;
        public ParticleSystem DryingUpBurst;

        public override void Awake()
        {
            base.Awake();
            golemManager.Initialize();
            collection.DEBUGLoadPlantProperties();
            gardenManager.RegisterGarden(areaIndex);
        }

        void OnEnable()
        {
            onTilemapSelection += GardenTileSelected;
            EventManager.instance.EVENT_PLANT_UPDATE += PlantUpdate;

            plantPrefabLookup = new Dictionary<Vector3Int, GameObject>();
            RefreshGarden();

            gardenManager.timer.onTick += (x) => DecrementWatering(x);
        }

        void OnDisable()
        {
            onTilemapSelection -= GardenTileSelected;
            EventManager.instance.EVENT_PLANT_UPDATE -= PlantUpdate;
        }

        void PlantUpdate(int areaIndex, Vector3Int cell)
        {
            if (areaIndex == this.areaIndex)
            {
                if (!gardenManager.IsEmpty(areaIndex, cell))
                {
                    DestroyPlantPrefabOnCell(cell);
                    PlacePlantPrefabOnCell(cell,
                        gardenManager.GetPlantType(areaIndex, cell),
                        gardenManager.GetGrowthStage(areaIndex, cell));
                }
            }
        }

        void RefreshGarden()
        {
            List<PlantState> plants = gardenManager.GetPlants(areaIndex);
            Debug.Log(plants);
            foreach (PlantState plant in plants)
            {
                PlantUpdate(areaIndex, plant.cell);
            }

            List<TileState> tileStates = gardenManager.GetTiles(areaIndex);
            foreach (TileState tileState in tileStates)
            {
                tilemap.SetTile(tileState.cell, tileStore[tileState.tileIndex].tile);
            }
        }
        public void ChangeGardenTile(Vector3Int position, TileData newTile)
        {
            tilemap.SetTile(position, newTile.tile);
            gardenManager.UpdateGardenTile(areaIndex, position, tileStore.tileDataSet.IndexOf(newTile));
        }

        public void ChangeGardenTileToGrass(Vector3Int position)
        {
            ChangeGardenTile(position, tileStore[0]);
        }

        public void ChangeGardenTileToPlot_Empty(Vector3Int position)
        {
            ChangeGardenTile(position, tileStore[1]);
        }

        public void ChangeGardenTileToPlot_Occupied(Vector3Int position)
        {
            ChangeGardenTile(position, tileStore[2]);
        }

        public void PlacePlantPrefabOnCell(Vector3Int cell, PlantId type, int growthStage)
        {
            Vector3 centerOfCell = tilemap.GetCellCenterWorld(cell);
            PlantProperties properties = collection.GetPlant(type);
            GameObject prefab = collection.GetPrefab(properties.spritePaths[growthStage]);
            plantPrefabLookup[cell] = Instantiate(prefab, centerOfCell, Quaternion.identity);
        }

        public void DestroyPlantPrefabOnCell(Vector3Int cell)
        {
            if (plantPrefabLookup.ContainsKey(cell))
            {
                Destroy(plantPrefabLookup[cell]);
                plantPrefabLookup.Remove(cell);
            }
        }

        public void UpdatePlantPrefabOnCell(PlantId type, Vector3Int cell)
        {
            DestroyPlantPrefabOnCell(cell);
            PlacePlantPrefabOnCell(cell, type, gardenManager.GetGrowthStage(areaIndex, cell));
        }

        public void CreatePlant(SeedId seed, Vector3Int cell)
        {
            PlantId type = collection.SeedToPlant(seed);

            if (!plantPrefabLookup.ContainsKey(cell) || gardenManager.IsEmpty(areaIndex, cell))
            {
                PlacePlantPrefabOnCell(cell, type, 0);
                gardenManager.CreatePlant(type, areaIndex, cell);
            }
        }

        public void DecrementWatering(int value)
        {
            List<PlantState> plants = gardenManager.GetPlants(areaIndex);

            foreach (PlantState plant in plants)
            {
                int waterTimerReset = collection.GetPlant(plant.type).growthTime;

                if (plant.waterTimer == 0)
                {
                    plantPrefabLookup[plant.cell].transform.localScale = Vector3.one * 0.9f;
                }
                else if (plant.waterTimer == (-1 * waterTimerReset))
                {
                    Vector3 centerOfCell = tilemap.GetCellCenterWorld(plant.cell);
                    Quaternion particleQuat = Quaternion.Euler(-110, 0, 0);
                    ParticleSystem PlayParticle = Instantiate(DryingUpBurst, centerOfCell + new Vector3(0, -1, -2), particleQuat);

                    plantPrefabLookup[plant.cell].transform.localScale = Vector3.one * 0.8f;
                }
                else if (plant.waterTimer == (-2 * waterTimerReset))
                {
                    gardenManager.DecrementWaterTimer(areaIndex, plant.cell, (value * -1));

                    Vector3 centerOfCell = tilemap.GetCellCenterWorld(plant.cell);
                    Quaternion particleQuat = Quaternion.Euler(-110, 0, 0);
                    ParticleSystem PlayParticle = Instantiate(DryingUpBurst, centerOfCell + new Vector3(0, -1, -2), particleQuat);

                    plantPrefabLookup[plant.cell].transform.localScale = Vector3.one * 0.6f;
                }

                //Debug.Log(plant.waterTimer);
                gardenManager.DecrementWaterTimer(areaIndex, plant.cell, (value * -1));
            }
        }

        public void WaterPlant(Vector3Int cell)
        {
            plantPrefabLookup[cell].transform.localScale = Vector3.one;

            if (gardenManager.UpdateWaterStage(areaIndex, cell))
            {
                PlantId type = gardenManager.GetPlantType(areaIndex, cell);

                if (gardenManager.UpdateGrowthStage(areaIndex, cell))
                {
                    UpdatePlantPrefabOnCell(type, cell);
                }
            }

            // Temporary code to spawn a particle system so we know that a plant has been fertilized
            // This should be replaced by some sort of animation/constant effect
            Vector3 centerOfCell = tilemap.GetCellCenterWorld(cell);
            Quaternion particleQuat = Quaternion.Euler(-110, 0, 0);
            ParticleSystem PlayParticle = Instantiate(WateringParticleBurst, centerOfCell + new Vector3(0, 0, -2), particleQuat);
        }

        public void FertilizePlant(Vector3Int cell)
        {
            if (gardenManager.SetFertilization(areaIndex, cell))
            {
                // Temporary code to spawn a particle system so we know that a plant has been fertilized
                // This should be replaced by some sort of animation/constant effect
                Vector3 centerOfCell = tilemap.GetCellCenterWorld(cell);
                Quaternion particleQuat = Quaternion.Euler(-110, 0, 0);
                ParticleSystem PlayParticle = Instantiate(FertilizerParticleBurst, centerOfCell + new Vector3(0, 0, -2), particleQuat);
            }
        }

        public bool HarvestPlantOnCell(Vector3Int cell)
        {
            if (gardenManager.GetGrowthStage(areaIndex, cell) == 2)
            {
                int numSeedsDropped = gardenManager.NumSeedDrops(areaIndex, cell);
                Debug.Log("Dropped " + numSeedsDropped + " Seeds.");

                EventManager.instance.HandleEVENT_GOLEM_SPAWN(0, cell);

                EventManager.instance.HandleEVENT_INVENTORY_ADD(
                    new Plant(2, "Tulip", 1, new Trait()), 1);
                DestroyPlantOnCell(cell);
                return true;
            }

            DestroyPlantOnCell(cell);
            return false;
        }

        public void DestroyPlantOnCell(Vector3Int cell)
        {
            DestroyPlantPrefabOnCell(cell);
            gardenManager.DestroyPlant(areaIndex, cell);
        }

        [ContextMenu("ClearPlants")]
        public void ClearPlants()
        {
            foreach (Vector3Int cell in plantPrefabLookup.Keys)
            {
                Destroy(plantPrefabLookup[cell]);
            }
            plantPrefabLookup = new Dictionary<Vector3Int, GameObject>();
            gardenManager.ClearGarden(areaIndex);
        }

        [ContextMenu("InspectPlants")]
        public void InspectPlants()
        {
            plantListDebug = gardenManager.GetPlants(areaIndex);
        }

        //Selects a garden tile in world space
        //Called through a Unity event as of now
        public void GardenTileSelected(Vector3Int gridPos)
        {
            playerController.QueueEntityAction(DoToolAction);
            playerTools.SetToolAction(tileStore[lastSelectedTile], lastSelectedCell, this);
        }

        void DoToolAction(EntityController entityController)
        {
            playerTools.DoCurrentToolAction();
        }

        /// <summary>
        /// Punnet square logic
        /// </summary>
        List<Genotype> punnetSquare;
        List<GardenPlant> neighbours;
        public void Harvest(BasePhenotypeData phenotypeData, Genotype genotype)
        {
            //NOTE: This would be calculated when the plant is placed down// when a tile near it is updated

            System.Type phenotype = phenotypeData.GetType();
            List<Genotype> daddies = new List<Genotype>();

            foreach (GardenPlant plant in neighbours)
            {
                if (plant.phenotypeData.GetType() != phenotype) continue;
                daddies.Add(plant.genotype);
            }
            //--------------------------------

            //Punnet square Row + Column

            Genotype daddyGenotype = daddies[Random.Range(0, daddies.Count)];    //NOTE: This should check weighting in the future

            int squareSide = (int)Mathf.Pow(2, phenotypeData.TraitCount);

            List<List<Allele>> topRow = new List<List<Allele>>();
            List<List<Allele>> leftCol = new List<List<Allele>>();

            //Pseudo bit array 
            int[] bitArray = new int[phenotypeData.TraitCount];
            int index;
            //Filling up the punnet square top/left headers
            for (int i = 0; i < squareSide; i++)
            {
                topRow.Add(new List<Allele>());
                leftCol.Add(new List<Allele>());

                for (int j = 0; j < phenotypeData.TraitCount; j++)
                {
                    index = bitArray[j];

                    if (index == 0)
                    {
                        topRow[i].Add(daddyGenotype[j].allele1);
                        leftCol[i].Add(genotype[j].allele1);
                    }
                    else
                    {
                        topRow[i].Add(daddyGenotype[j].allele2);
                        leftCol[i].Add(genotype[j].allele2);
                    }
                }

                //Incrementing our pseudo binary number (int array)
                bitArray[0] += 1;
                for (int k = 0; k < phenotypeData.TraitCount - 1; k++)
                {
                    if (bitArray[k] > 1)
                    {
                        bitArray[k + 1] += 1;
                        bitArray[k] = 0;
                    }
                }
            }

            //Storing the actual combinations
            punnetSquare = new List<Genotype>();
            Genotype squareValue = null;
            for (int y = 0; y < squareSide; y++)
            {
                for (int x = 0; x < squareSide; x++)
                {
                    squareValue = new Genotype();

                    for (int i = 0; i < phenotypeData.TraitCount; i++)
                    {
                        Trait trait;
                        trait.allele1 = topRow[x][i];
                        trait.allele2 = leftCol[y][i];

                        squareValue[i] = trait;
                    }

                    punnetSquare.Add(squareValue);
                }
            }

            Genotype childGenotype = punnetSquare[Random.Range(0, punnetSquare.Count)];

            ITraitSetData[] traitList = new ITraitSetData[phenotypeData.TraitCount];

            for (int i = 0; i < phenotypeData.TraitCount; i++)
            {
                traitList[i] = phenotypeData.TraitList[i];

                //traitList[i].dominant.
                if (childGenotype[i].allele1 == Allele.dominant && childGenotype[i].allele2 == Allele.dominant)
                {
                    //traits[i] = phenotypeData.TraitList[i];
                }
                else if (childGenotype[i].allele1 == Allele.recessive && childGenotype[i].allele2 == Allele.recessive)
                {
                    //traits[i] = phenotypeData.TraitList[i];
                }
                else
                {
                    // traits[i] = phenotypeData.TraitList[i];
                }
            }
        }
    }
}
