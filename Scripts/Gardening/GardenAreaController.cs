using GrandmaGreen.Collections;
using GrandmaGreen.Entities;
using GrandmaGreen.TimeLayer;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        public GardenCustomizer gardenCustomizer;
        public GameObject defaultPlantPrefab;
        public Dictionary<Vector3Int, GameObject> plantPrefabLookup;
        public List<Collider> decorList;
        public List<PlantState> plantListDebug;
        public TileStore tileStore;

        [Header("Golem Management")]
        public GolemManager golemManager;

        [Header("Temporary Effects")]
        public bool DEBUG_CUSTOMIZATION;
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
            decorList = new List<Collider>();
            RefreshGarden();

            gardenManager.timer.onTick += (x) => DecrementWatering(x);
        }

        void OnDisable()
        {
            onTilemapSelection -= GardenTileSelected;
            EventManager.instance.EVENT_PLANT_UPDATE -= PlantUpdate;

            gardenManager.timer.onTick -= (x) => DecrementWatering(x);
        }

        #region Plants
        void PlantUpdate(int areaIndex, Vector3Int cell)
        {
            if (areaIndex == this.areaIndex)
            {
                if (!gardenManager.IsEmpty(areaIndex, cell))
                {
                    UpdateSprite(cell);    
                }
            }
        }

        void RefreshGarden()
        {
            List<PlantState> plants = gardenManager.GetPlants(areaIndex);
            foreach (PlantState plant in plants)
            {
                InstantiatePlantPrefab(plant.cell, plant.type, plant.growthStage);
            }

            List<TileState> tileStates = gardenManager.GetTiles(areaIndex);
            foreach (TileState tileState in tileStates)
            {
                tilemap.SetTile(tileState.cell, tileStore[tileState.tileIndex].tile);
            }
        }

        public void UpdateSprite(Vector3Int cell)
        {
            Transform transform = plantPrefabLookup[cell].transform;
            SpriteRenderer spriteRenderer = (SpriteRenderer)transform.Find("Sprite3D")
                .GetComponent(typeof(SpriteRenderer));
            PlantState plant = gardenManager.GetPlant(areaIndex, cell);
            Genotype genotype = gardenManager.GetGenotype(areaIndex, cell);
            Sprite sprite = collection.GetSprite(plant.type, genotype, plant.growthStage);
            spriteRenderer.sprite = sprite;
            // TODO: remove hard coded scaling
            transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        }

        public void InstantiatePlantPrefab(Vector3Int cell, PlantId type, int growthStage)
        {
            // TODO: remove hard coded offset
            Vector3 centerOfCell = tilemap.GetCellCenterWorld(cell) + (0.5f * Vector3.back);
            plantPrefabLookup[cell] = Instantiate(defaultPlantPrefab, centerOfCell, Quaternion.identity);
            UpdateSprite(cell);
        }

        public void DestroyPlantPrefab(Vector3Int cell)
        {
            if (plantPrefabLookup.ContainsKey(cell))
            {
                Destroy(plantPrefabLookup[cell]);
                plantPrefabLookup.Remove(cell);
            }
        }

        public void CreatePlant(SeedId seed, Genotype genotype, Vector3Int cell, int growthStage=0)
        {
            PlantId type = collection.SeedToPlant(seed);

            if (!plantPrefabLookup.ContainsKey(cell) || gardenManager.IsEmpty(areaIndex, cell))
            {
                gardenManager.CreatePlant(type, genotype, growthStage, areaIndex, cell);
                InstantiatePlantPrefab(cell, type, growthStage);
            }
        }

        public void DecrementWatering(int value)
        {
            List<PlantState> plants = gardenManager.GetPlants(areaIndex);

            foreach (PlantState plant in plants)
            {
                if (gardenManager.GetGrowthStage(areaIndex, plant.cell) != 2) {
                    int waterTimerReset = collection.GetPlant(plant.type).growthTime;

                    if (plant.waterTimer >= (-2 * waterTimerReset)) {
                        // This section is all Temporary Code until we find a different visual solution
                        // for showing the state of a plant.
                        /*if (plant.waterTimer == waterTimerReset)
                        {
                            plantPrefabLookup[plant.cell].transform.localScale = Vector3.one;
                        }
                        else if (plant.waterTimer == 0)
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
                            Vector3 centerOfCell = tilemap.GetCellCenterWorld(plant.cell);
                            Quaternion particleQuat = Quaternion.Euler(-110, 0, 0);
                            ParticleSystem PlayParticle = Instantiate(DryingUpBurst, centerOfCell + new Vector3(0, -1, -2), particleQuat);

                            plantPrefabLookup[plant.cell].transform.localScale = Vector3.one * 0.6f;
                        }*/

                        Debug.Log(plant.waterTimer);
                        gardenManager.DecrementWaterTimer(areaIndex, plant.cell, (value * -1));
                    }
                }
            }
        }

        public void WaterPlant(Vector3Int cell)
        {
            if (gardenManager.UpdateWaterStage(areaIndex, cell))
            {
                PlantId type = gardenManager.GetPlantType(areaIndex, cell);

                if (gardenManager.UpdateGrowthStage(areaIndex, cell))
                {
                    UpdateSprite(cell);
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

        public bool HarvestPlant(Vector3Int cell)
        {
            if (gardenManager.IsEmpty(areaIndex, cell)) return false;

            int maxGrowthStages = collection.GetPlant(gardenManager
		        .GetPlantType(areaIndex, cell)).growthStages;

            if (gardenManager.PlantIsFullyGrown(areaIndex, cell))
            {
                Debug.Log("Identifying breeding candidates - ");
                List<Genotype> breedingCandidates = new List<Genotype>();
                foreach (Vector3Int neighbor in new[] { cell + Vector3Int.up, cell + Vector3Int.down,
		            cell + Vector3Int.left, cell + Vector3Int.right})
                {
                    if (gardenManager.PlantIsBreedable(areaIndex, neighbor))
                    {
                        Debug.Log("Plant is breedable " + neighbor);
                        breedingCandidates.Add(gardenManager.GetGenotype(areaIndex, neighbor));
                    }
                }

                PlantId motherPlantType = gardenManager.GetPlantType(areaIndex, cell);
                Genotype motherGenotype = gardenManager.GetGenotype(areaIndex, cell);
                PlantId childPlantType = motherPlantType;
                Genotype childGenotype = motherGenotype;
                
                if (breedingCandidates.Count != 0)
                {
                    Genotype fatherGenotype = breedingCandidates[Random.Range(0, breedingCandidates.Count)];
                    childGenotype = motherGenotype.Cross(fatherGenotype);
                    Debug.Log("Breeding occured.");
                    Debug.Log("Father genotype: " + fatherGenotype);
                }
                else
                    Debug.Log("No breeding occured.");

                Debug.Log("Mother genotype: " + motherGenotype);
                Debug.Log("Child genotype: " + childGenotype);

                int numSeedsDropped = gardenManager.NumSeedDrops(areaIndex, cell);
                
                PlantProperties properties = collection.GetPlant(childPlantType);
                EventManager.instance.HandleEVENT_INVENTORY_ADD_PLANT_OR_SEED(
                    new Plant((ushort)motherPlantType, properties.name, 1, new List<Genotype> { motherGenotype }), motherGenotype);
                EventManager.instance.HandleEVENT_INVENTORY_ADD_PLANT_OR_SEED(
                    new Seed((ushort)collection.PlantToSeed(childPlantType), properties.name, new List<Genotype> { childGenotype }), childGenotype);
                EventManager.instance.HandleEVENT_GOLEM_SPAWN(0, cell);

                DestroyPlant(cell);
		        return true;
            }

            DestroyPlant(cell);
            return false;
        }

        public void DestroyPlant(Vector3Int cell)
        {
            DestroyPlantPrefab(cell);
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

        [ContextMenu("ToggleGrowthTimer")]
        public void ToggleGrowthTimer()
        {
            if (!gardenManager.timer.paused)
            {
                gardenManager.timer.Pause();
                Debug.Log("Paused Growth Timer");
            }
            else
            {
                gardenManager.timer.Resume();
                Debug.Log("Resumed Growth Timer");
            }
        }

        [ContextMenu("CrossBreedTest")]
        public void CrossBreedTest()
	    {
            DestroyPlant(Vector3Int.zero);
            DestroyPlant(Vector3Int.up);
            DestroyPlant(Vector3Int.down);
            DestroyPlant(Vector3Int.left);
            DestroyPlant(Vector3Int.right);
            ChangeGardenTileToPlot_Occupied(Vector3Int.zero);
            ChangeGardenTileToPlot_Occupied(Vector3Int.up);
            ChangeGardenTileToPlot_Occupied(Vector3Int.down);
            ChangeGardenTileToPlot_Occupied(Vector3Int.left);
            ChangeGardenTileToPlot_Occupied(Vector3Int.right);
            CreatePlant(SeedId.Tulip, new Genotype("AaBb"), Vector3Int.zero, 2);
            CreatePlant(SeedId.Rose, new Genotype("AABB"), Vector3Int.up, 2);
            CreatePlant(SeedId.Rose, new Genotype("aabb"), Vector3Int.down, 2);
            CreatePlant(SeedId.Rose, new Genotype("aAbB"), Vector3Int.left, 2);
            CreatePlant(SeedId.Rose, new Genotype("AaBb"), Vector3Int.right, 2);
        }
        #endregion

        #region Tiles

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

        #endregion
        
        
        //Selects a garden tile in world space
        //Called through a Unity event as of now
        public void GardenTileSelected(Vector3Int gridPos)
        {
            if(DEBUG_CUSTOMIZATION)
            {
                EnterCustomizationMode();
                playerController.CancelDestination();
                return;
            }

            playerController.QueueEntityAction(DoToolAction);
            playerTools.SetToolAction(tileStore[lastSelectedTile], lastSelectedCell, this);
        }

        void DoToolAction(EntityController entityController)
        {
            playerTools.DoCurrentToolAction();
        }
        
        public void EnterCustomizationMode()
        {
            BoxCollider decorItem = gardenCustomizer.GenerateDecorItem();

            gardenCustomizer.EnterCustomizationState(this, decorItem);
        }

        public void AddDecorItem(BoxCollider decorItem)
        {
            Debug.Log("Decor item added!");

            //Save decor item here
            //Adjust pathfinding grid here
        }

        /*
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
        */
    }
    
}
