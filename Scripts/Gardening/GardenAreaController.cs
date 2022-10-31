using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GrandmaGreen.Entities;
using GrandmaGreen.Collections;

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

        [Header("Plant Management")]
        //public PlantTypeDictionary plantTypeDictionary;
        public PlantStateManager plantStateManager;
        public Dictionary<Vector3Int, GameObject> plantPrefabLookup;
        public List<PlantState> plantListDebug;

        [Header("Golem Management")]
        public GolemManager golemManager;

        [Header("Temporary Effects")]
        public ParticleSystem FertilizerParticleBurst;
        public ParticleSystem WateringParticleBurst;

        public override void Awake()
        {
            base.Awake();
            golemManager.Initialize();
            plantStateManager.Initialize();
            plantStateManager.RegisterGarden(areaIndex);
            collection.DEBUGLoadPlantProperties();
        }

        public override void Activate()
        {
            plantPrefabLookup = new Dictionary<Vector3Int, GameObject>();
            base.Activate();
        }

        void OnEnable()
        {
            onTilemapSelection += GardenTileSelected;
            EventManager.instance.EVENT_PLANT_UPDATE += PlantUpdate;
        }

        void OnDisable()
        {
            onTilemapSelection -= GardenTileSelected;
            EventManager.instance.EVENT_PLANT_UPDATE -= PlantUpdate;
        }

        /*IEnumerator GrowthCoroutine(Vector3Int cell)
        {
            PlantId type = plantStateManager.GetPlantType(areaIndex, cell);
            PlantProperties properties = collection.GetPlant(type);

            Debug.Log(string.Format("Growing {0} at {1}.", properties.name, cell));
            for (int i = 1; i < properties.growthStages; i++)
            {
                yield return new WaitForSeconds(5);

                if (plantStateManager.IsEmpty(areaIndex, cell))
                    yield break;

                plantStateManager.IncrementGrowthStage(areaIndex, cell);
                UpdatePlantPrefabOnCell(type, cell);
                Debug.Log(string.Format("{0} grew. Current stage: {1}", properties.name, plantStateManager.GetGrowthStage(areaIndex, cell)));
                InspectPlants();
            }
            Debug.Log("Done growing.");
        }*/

        void PlantUpdate(int areaIndex, Vector3Int cell)
        {
            if (areaIndex == this.areaIndex)
            {
                Debug.Log(string.Format("Updating plant in garden {0} at {1}", areaIndex, cell));
                if (!plantStateManager.IsEmpty(areaIndex, cell))
                {
                    DestroyPlantPrefabOnCell(cell);
                    PlacePlantPrefabOnCell(cell,
                        plantStateManager.GetPlantType(areaIndex, cell),
                        plantStateManager.GetGrowthStage(areaIndex, cell));
                }
            }
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
            PlacePlantPrefabOnCell(cell, type, plantStateManager.GetGrowthStage(areaIndex, cell));
        }

        public void CreatePlant(SeedId seed, Vector3Int cell)
        {
            PlantId type = collection.SeedToPlant(seed);

            if (!plantPrefabLookup.ContainsKey(cell) || plantStateManager.IsEmpty(areaIndex, cell))
            {
                PlacePlantPrefabOnCell(cell, type, 0);
                plantStateManager.CreatePlant(type, areaIndex, cell);
                //StartCoroutine(GrowthCoroutine(cell));
            }
        }

        public void WaterPlant(Vector3Int cell)
        {
            if(plantStateManager.UpdateWaterStage(areaIndex, cell))
            {
                PlantId type = plantStateManager.GetPlantType(areaIndex, cell);

                if(plantStateManager.UpdateGrowthStage(areaIndex, cell))
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
            if(plantStateManager.SetFertilization(areaIndex, cell))
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
            if (plantStateManager.GetGrowthStage(areaIndex, cell) == 2)
            {
                int numSeedsDropped = plantStateManager.NumSeedDrops(areaIndex, cell);
                Debug.Log("Dropped " + numSeedsDropped + " Seeds.");

                EventManager.instance.HandleEVENT_GOLEM_SPAWN(0, cell);

                EventManager.instance.HandleEVENT_INVENTORY_ADD(
                    new GrandmaGreen.Collections.Plant(2, "Tulip", 1, new Trait()), 1);
                DestroyPlantOnCell(cell);
                return true;
            }

            DestroyPlantOnCell(cell);
            return false;
        }

        public void DestroyPlantOnCell(Vector3Int cell)
        {
            DestroyPlantPrefabOnCell(cell);
            plantStateManager.DestroyPlant(areaIndex, cell);
        }

        [ContextMenu("DestroyAllPlants")]
        public void DestroyAllPlants()
        {
            foreach (Vector3Int cell in plantPrefabLookup.Keys)
            {
                Destroy(plantPrefabLookup[cell]);
            }
            plantPrefabLookup = new Dictionary<Vector3Int, GameObject>();
            plantStateManager.ClearGarden(areaIndex);
        }

        [ContextMenu("InspectPlants")]
        public void InspectPlants()
        {
            plantListDebug = plantStateManager.GetPlants(areaIndex);
        }

        //Selects a garden tile in world space
        //Called through a Unity event as of now
        public void GardenTileSelected(Vector3Int gridPos)
        {
            playerController.QueueEntityAction(DoToolAction);
            playerTools.SetToolAction(lastSelectedTile, lastSelectedCell, this);
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
