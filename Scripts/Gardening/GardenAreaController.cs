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
        public Cinemachine.CinemachineVirtualCamera customizationCamera;
        public GameObject defaultPlantPrefab;
        public Dictionary<Vector3Int, GameObject> plantPrefabLookup;
        public List<Collider> decorList;
        public List<PlantState> plantListDebug;
        public TileStore tileStore;

        bool returnFromPause = true;

        [Header("Temporary Effects")]
        public bool m_inCustomizationMode = false;
        public ParticleSystem FertilizerParticleBurst;
        public ParticleSystem WateringParticleBurst;
        public ParticleSystem DryingUpBurst;

        public override void Awake()
        {
            base.Awake();
            collection.DEBUGLoadPlantProperties();
            gardenManager.RegisterGarden(areaIndex);

            gardenManager.timers[areaIndex].Pause();
        }

        void OnEnable()
        {
            gardenManager.timers[areaIndex].Resume(true);
            gardenManager.timers[areaIndex].onTick += IncrementTimer;

            onTilemapSelection += GardenTileSelected;
            EventManager.instance.EVENT_PLANT_UPDATE += PlantUpdate;

            EventManager.instance.EVENT_INVENTORY_CUSTOMIZATION_START += StartDecorCustomization;
            EventManager.instance.EVENT_CUSTOMIZATION_ATTEMPT += CompleteDecorCustomization;

            EventManager.instance.EVENT_TOGGLE_CUSTOMIZATION_MODE += ToggleCustomizationMode;

            plantPrefabLookup = new Dictionary<Vector3Int, GameObject>();
            decorList = new List<Collider>();
            RefreshGarden();

            EventManager.instance.HandleEVENT_INVENTORY_ADD_DECOR((ushort)DecorationId.Flamingo);
        }

        void OnDisable()
        {
            onTilemapSelection -= GardenTileSelected;
            EventManager.instance.EVENT_PLANT_UPDATE -= PlantUpdate;

            EventManager.instance.EVENT_INVENTORY_CUSTOMIZATION_START -= StartDecorCustomization;
            EventManager.instance.EVENT_CUSTOMIZATION_ATTEMPT -= CompleteDecorCustomization;

            EventManager.instance.EVENT_TOGGLE_CUSTOMIZATION_MODE -= ToggleCustomizationMode;

            gardenManager.timers[areaIndex].Pause();
            gardenManager.timers[areaIndex].onTick -= IncrementTimer;
            returnFromPause = true;
        }

        #region Input

        public override void ProcessAreaInput(InteractionEventData eventData)
        {
            if (eventData.interactionState.phase == PointerState.Phase.DOWN && !m_inCustomizationMode)
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

                decorItem.onInteraction += StartDecorCustomization;
                decorItem.DisableInteraction();
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

            if (gardenManager.PlantIsWilted(areaIndex, cell))
            {
                spriteRenderer.color = new Color(0.71f, 0.4f, 0.11f);
            }
            else if (gardenManager.PlantIsDead(areaIndex, cell))
            {
                spriteRenderer.color = new Color(0.40f, 0.26f, 0.13f);
            }
            else
            {
                spriteRenderer.color = new Color(1f, 1f, 1f);
            }
            // TODO: remove hard coded scaling
            transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        }

        public void InstantiatePlantPrefab(Vector3Int cell, PlantId type, int growthStage)
        {
            Vector3 centerOfCell = tilemap.GetCellCenterWorld(cell);
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
            if (!plantPrefabLookup.ContainsKey(cell) || gardenManager.IsEmpty(areaIndex, cell))
            {
                gardenManager.CreatePlant(seed, genotype, growthStage, areaIndex, cell);
                InstantiatePlantPrefab(cell, seed, growthStage);
                // Remove the seed from inventory.
                EventManager.instance.HandleEVENT_INVENTORY_REMOVE_SEED((ushort)seed, genotype);
            }
        }

        public void IncrementTimer(int value)
        {
            List<PlantState> plantList = gardenManager.GetPlants(areaIndex);

            foreach (PlantState plant in plantList)
            {
                gardenManager.IncrementWaterTimer(areaIndex, plant.cell, value);

                if (plant.waterTimer == collection.GetPlant(plant.type).growthTime)
                {
                    Debug.Log("Plant is ready for water");
                }
                else if (plant.waterTimer == gardenManager.WiltTime
                    || (gardenManager.PlantIsWilted(areaIndex, plant.cell) && returnFromPause)) //>= 240)
                {
                    // Wilted
                    // Debug.Log("Plant is wilted");
                    UpdateSprite(plant.cell);

                    Vector3 centerOfCell = tilemap.GetCellCenterWorld(plant.cell);
                    Quaternion particleQuat = Quaternion.Euler(-110, 0, 0);
                    ParticleSystem PlayParticle = Instantiate(DryingUpBurst, centerOfCell + new Vector3(0, -1, -2), particleQuat);
                }
                else if (!gardenManager.PlantIsFullyGrown(areaIndex, plant.cell))
                {
                    if (plant.waterTimer == gardenManager.DeathTime
                        || (gardenManager.PlantIsDead(areaIndex, plant.cell) && returnFromPause)) //>= 336)
                    {
                        // Dead
                        // Debug.Log("Plant is dead");
                        UpdateSprite(plant.cell);

                        Vector3 centerOfCell = tilemap.GetCellCenterWorld(plant.cell);
                        Quaternion particleQuat = Quaternion.Euler(-110, 0, 0);
                        ParticleSystem PlayParticle = Instantiate(DryingUpBurst, centerOfCell + new Vector3(0, -1, -2), particleQuat);
                    }
                }

                if (plant.waterStage == 1
                    && plant.waterTimer >= collection.GetPlant(plant.type).growthTime)
                {
                    // Debug.Log("Plant Growth via IncrementWaterTimer");
                    gardenManager.UpdateGrowthStage(areaIndex, plant.cell);

                    UpdateSprite(plant.cell);
                }

                if (returnFromPause) UpdateSprite(plant.cell);
            }

            if (returnFromPause) returnFromPause = false;
        }

        public void WaterPlant(Vector3Int cell)
        {
            if (!gardenManager.PlantIsDead(areaIndex, cell))
            {
                if (gardenManager.UpdateWaterStage(areaIndex, cell))
                {
                    gardenManager.UpdateGrowthStage(areaIndex, cell);
                    // Debug.Log("Plant Growth via WaterPlant");
                }
                UpdateSprite(cell);
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

            string debug = "Breeding Candidate Debug (Click to Expand)\n";

            if (gardenManager.PlantIsBreedable(areaIndex, cell))
            {
                debug += "Breeding candidates: ";
                List<Genotype> breedingCandidates = new List<Genotype>();
                foreach (Vector3Int neighbor in new[] { cell + Vector3Int.up, cell + Vector3Int.down,
                    cell + Vector3Int.left, cell + Vector3Int.right})
                {
                    if (gardenManager.PlantIsBreedable(areaIndex, neighbor))
                    {
                        breedingCandidates.Add(gardenManager.GetGenotype(areaIndex, neighbor));
                        debug += neighbor + " ";
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
                    debug += "\nBreeding occured.";
                }
                else
                    debug += "\nNo breeding occured.";

                Debug.Log(debug);

                int numSeedsDropped = gardenManager.NumSeedDrops(areaIndex, cell);

                PlantProperties properties = collection.GetPlant(childPlantType);
                EventManager.instance.HandleEVENT_INVENTORY_ADD_PLANT((ushort)motherPlantType, motherGenotype);
                EventManager.instance.HandleEVENT_INVENTORY_ADD_SEED((ushort)childPlantType, childGenotype);

                DestroyPlant(cell);

                //EventManager.instance.HandleEVENT_GOLEM_SPAWN(0, cell);
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
            if (!gardenManager.timers[areaIndex].paused)
            {
                gardenManager.timers[areaIndex].Pause();
                Debug.Log("Paused Growth Timer");
            }
            else
            {
                gardenManager.timers[areaIndex].Resume();
                Debug.Log("Resumed Growth Timer");
            }
        }

        [ContextMenu("CrossBreedTest")]
        public void CrossBreedTest()
        {
            Vector3Int right = Vector3Int.zero + 3 * Vector3Int.up;
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

        [ContextMenu("GenotypeTest")]
        public void GenotypeTest()
        {
            Genotype first = new Genotype("AaBb");
            Genotype second = new Genotype("AaBb");
            first.Cross(second);
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

        public void ToggleCustomizationMode()
        {
            if (m_inCustomizationMode) ExitCustomizationMode();
            else EnterCustomizationMode();
        }

        public void EnterCustomizationMode()
        {
            m_inCustomizationMode = true;
            customizationCamera.gameObject.SetActive(true);
            playerController.entity.gameObject.SetActive(false);
        }

        public void ExitCustomizationMode()
        {
            m_inCustomizationMode = false;
            customizationCamera.gameObject.SetActive(false);
            playerController.entity.gameObject.SetActive(true);
        }

        public void StartDecorCustomization(IInventoryItem item)
        {
            GardenDecorItem decorItem = gardenCustomizer.GenerateDecorItem((DecorationId)item.itemID);
            m_IsGeneratedDecorItem = true;

            if (!m_inCustomizationMode) EventManager.instance.HandleEVENT_TOGGLE_CUSTOMIZATION_MODE();

            StartDecorCustomization(decorItem);
        }

        public void StartDecorCustomization(GardenDecorItem decorItem)
        {
            m_CurrentDecorItem = decorItem;
            m_OriginalPosition = m_CurrentDecorItem.transform.position;
            lastDraggedPosition = m_OriginalPosition;
            gardenCustomizer.EnterDecorCustomizationState(this, m_CurrentDecorItem);
        }

        public void CompleteDecorCustomization(bool successful)
        {
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

            decorItem.onInteraction += StartDecorCustomization;
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
