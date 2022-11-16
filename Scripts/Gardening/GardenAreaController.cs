using Core.Input;
using GrandmaGreen.Collections;
using GrandmaGreen.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Garden
{
    /// <summary>
    /// Responsible for functionality of a single Garden screen
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
        public bool DEBUG_CUSTOMIZATION = false;
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

            EventManager.instance.EVENT_CUSTOMIZATION_START += EnterCustomizationMode;
            EventManager.instance.EVENT_CUSTOMIZATION_END += ExitCustomizationMode;

            plantPrefabLookup = new Dictionary<Vector3Int, GameObject>();
            decorList = new List<Collider>();
            RefreshGarden();

            EventManager.instance.HandleEVENT_INVENTORY_ADD_PLANT_OR_SEED(new GrandmaGreen.Collections.Plant((ushort)DecorationId.Flamingo, null, 0, new List<Genotype>()), new Genotype());
            
            gardenManager.timer.onTick += DecrementWatering;
        }

        void OnDisable()
        {
            onTilemapSelection -= GardenTileSelected;
            EventManager.instance.EVENT_PLANT_UPDATE -= PlantUpdate;

            EventManager.instance.EVENT_CUSTOMIZATION_START -= EnterCustomizationMode;
            EventManager.instance.EVENT_CUSTOMIZATION_END -= ExitCustomizationMode;

            gardenManager.timer.onTick -= DecrementWatering;
        }

        #region Input

        public override void ProcessAreaInput(InteractionEventData eventData)
        {
            if (eventData.interactionState.phase == PointerState.Phase.DOWN && !DEBUG_CUSTOMIZATION)
            {
                AreaSelection(eventData.interactionPoint);
            }
            else if (eventData.interactionState.phase == PointerState.Phase.DRAG)
            {
                AreaDragged(eventData.interactionPoint);
            }
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

        #endregion

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

            List<DecorState> decorStates = gardenManager.GetDecor(areaIndex);
            foreach (DecorState decorState in decorStates)
            {
                GardenDecorItem decorItem = gardenCustomizer.GenerateDecorItem(decorState.ID);
                decorItem.transform.position = new Vector3(decorState.x, decorState.y, 0);

                decorItem.onInteraction += EnterCustomizationMode;
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

        public void CreatePlant(PlantId seed, Genotype genotype, Vector3Int cell, int growthStage = 0)
        {
            PlantId type = seed - CSVtoSO.SEED_ID_OFFSET;

            if (!plantPrefabLookup.ContainsKey(cell) || gardenManager.IsEmpty(areaIndex, cell))
            {
                gardenManager.CreatePlant(type, genotype, growthStage, areaIndex, cell);
                InstantiatePlantPrefab(cell, seed, growthStage);
            }
        }

        public void DecrementWatering(int value)
        {
            List<PlantState> plants = gardenManager.GetPlants(areaIndex);

            foreach (PlantState plant in plants)
            {
                if (gardenManager.GetGrowthStage(areaIndex, plant.cell) != 2)
                {
                    int waterTimerReset = collection.GetPlant(plant.type).growthTime;

                    if (plant.waterTimer >= (-2 * waterTimerReset))
                    {
                        // This section is all Temporary Code until we find a different visual solution
                        // for showing the state of a plant.
                        if (plant.waterTimer == (-1 * waterTimerReset))
                        {
                            Vector3 centerOfCell = tilemap.GetCellCenterWorld(plant.cell);
                            Quaternion particleQuat = Quaternion.Euler(-110, 0, 0);
                            ParticleSystem PlayParticle = Instantiate(DryingUpBurst, centerOfCell + new Vector3(0, -1, -2), particleQuat);
                        }
                        else if (plant.waterTimer == (-2 * waterTimerReset))
                        {
                            Vector3 centerOfCell = tilemap.GetCellCenterWorld(plant.cell);
                            Quaternion particleQuat = Quaternion.Euler(-110, 0, 0);
                            ParticleSystem PlayParticle = Instantiate(DryingUpBurst, centerOfCell + new Vector3(0, -1, -2), particleQuat);
                        }

                        //Debug.Log(plant.waterTimer);
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
                    new Seed((ushort)childPlantType, properties.name, new List<Genotype> { childGenotype }), childGenotype);
                DestroyPlant(cell);

                EventManager.instance.HandleEVENT_GOLEM_SPAWN(0, cell);
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
            CreatePlant(PlantId.Tulip, new Genotype("AaBb"), Vector3Int.zero, 2);
            CreatePlant(PlantId.Rose, new Genotype("AABB"), Vector3Int.up, 2);
            CreatePlant(PlantId.Rose, new Genotype("aabb"), Vector3Int.down, 2);
            CreatePlant(PlantId.Rose, new Genotype("aAbB"), Vector3Int.left, 2);
            CreatePlant(PlantId.Rose, new Genotype("AaBb"), Vector3Int.right, 2);

            Vector3Int right = Vector3Int.zero + 5 * Vector3Int.right + 5 * Vector3Int.up;
            for (int i = 0; i <= 6; i += 2)
            {
                DestroyPlant(right + i * Vector3Int.down);
                DestroyPlant(right + i * Vector3Int.down + Vector3Int.right);
                ChangeGardenTileToPlot_Occupied(right + i * Vector3Int.down);
                ChangeGardenTileToPlot_Occupied(right + i * Vector3Int.down + Vector3Int.right);
                CreatePlant(PlantId.Tulip, new Genotype("AaBb"), right + i * Vector3Int.down, 2);
            }
            CreatePlant(PlantId.Rose, new Genotype("aabb"), right + 0 * Vector3Int.down + Vector3Int.right, 2);
            CreatePlant(PlantId.Rose, new Genotype("AaBb"), right + 2 * Vector3Int.down + Vector3Int.right, 2);
            CreatePlant(PlantId.Rose, new Genotype("aAbB"), right + 4 * Vector3Int.down + Vector3Int.right, 2);
            CreatePlant(PlantId.Rose, new Genotype("AABB"), right + 6 * Vector3Int.down + Vector3Int.right, 2);
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

        #region  Customziation

        GardenDecorItem m_CurrentDecorItem;
        bool m_IsGeneratedDecorItem = false;
        Vector3 m_OriginalPosition;
        public void EnterCustomizationMode(IInventoryItem item)
        {
            GardenDecorItem decorItem = gardenCustomizer.GenerateDecorItem();

            m_IsGeneratedDecorItem = true;

            EnterCustomizationMode(decorItem);
        }

        public void EnterCustomizationMode(GardenDecorItem decorItem)
        {
            DEBUG_CUSTOMIZATION = true;
            m_CurrentDecorItem = decorItem;
            m_OriginalPosition = m_CurrentDecorItem.transform.position;
            lastDraggedPosition = m_OriginalPosition;

            gardenCustomizer.EnterCustomizationState(this, m_CurrentDecorItem);
        }

        public void ExitCustomizationMode(bool successful)
        {
            DEBUG_CUSTOMIZATION = false;

            if (successful)
            {
                if (m_IsGeneratedDecorItem)
                {
                    AddGardenDecorItem(m_CurrentDecorItem);
                }
                else
                {
                    UpdateGardenDecorItem(m_CurrentDecorItem);
                }
            }
            else
            {
                if (m_IsGeneratedDecorItem)
                {
                    Destroy(m_CurrentDecorItem.gameObject);
                }
                else
                    m_CurrentDecorItem.transform.position = m_OriginalPosition;
            }

            m_IsGeneratedDecorItem = false;
        }

        public void AddGardenDecorItem(GardenDecorItem decorItem)
        {
            gardenManager.UpdateDecorItem(areaIndex, decorItem.decorID, decorItem.transform.position);

            decorItem.onInteraction += EnterCustomizationMode;
        }

        public void UpdateGardenDecorItem(GardenDecorItem decorItem)
        {
            gardenManager.UpdateDecorItem(areaIndex, decorItem.decorID, decorItem.transform.position, m_OriginalPosition);
        }

        //TODO - 
        public void RemoveDecorItemFromGarden(GardenDecorItem decorItem)
        {
            Debug.Log("Decor item removed!");
        }

        #endregion
    }
}
