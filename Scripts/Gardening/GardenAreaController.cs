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
        public GardenVFX gardenVFX;
        public Cinemachine.CinemachineVirtualCamera customizationCamera;
        public GameObject defaultFlowerPrefab;
        public GameObject defaultVegetablePrefab;
        public GameObject defaultFruitPrefab;
        public Dictionary<Vector3Int, GameObject> plantPrefabLookup;
        public List<Collider> decorList;
        public List<PlantState> plantListDebug;
        public List<PlantState> wiltedPlantList;
        public TileStore tileStore;

        bool returnFromPause = true;

        [Header("Temporary Effects")]
        public bool m_inCustomizationMode = false;
        public float DropRate = 100;
        public float FertilizationBonus = 10;

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
            EventManager.instance.EVENT_WATER_PLANT += WaterPlant;

            EventManager.instance.EVENT_INVENTORY_CUSTOMIZATION_START += StartDecorCustomization;
            EventManager.instance.EVENT_CUSTOMIZATION_ATTEMPT += CompleteDecorCustomization;

            EventManager.instance.EVENT_TOGGLE_CUSTOMIZATION_MODE += ToggleCustomizationMode;

            plantPrefabLookup = new Dictionary<Vector3Int, GameObject>();
            decorList = new List<Collider>();
            RefreshGarden();
        }

        void OnDisable()
        {
            onTilemapSelection -= GardenTileSelected;
            EventManager.instance.EVENT_PLANT_UPDATE -= PlantUpdate;
            EventManager.instance.EVENT_WATER_PLANT -= WaterPlant;

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

        public PlantState GetPlant(Vector3Int cell)
        {
            if (!gardenManager.IsEmpty(areaIndex, cell))
            {
                return gardenManager.GetPlant(areaIndex, cell);
            }
            return new PlantState();
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

        public void UpdateFlowerSprite(Vector3Int cell)
        {
            Transform flowerTransform = plantPrefabLookup[cell].transform;
            SpriteRenderer spriteRenderer = flowerTransform.Find("Sprite3D").GetComponent<SpriteRenderer>();
            PlantState plant = gardenManager.GetPlant(areaIndex, cell);
            Genotype genotype = gardenManager.GetGenotype(areaIndex, cell);
            spriteRenderer.sprite = collection.GetSprite(plant.type, genotype, plant.growthStage);
            flowerTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * plant.genotype.SpriteSize();

            if (gardenManager.PlantIsWilted(areaIndex, cell))
            {
                spriteRenderer.color = new Color(0.71f, 0.4f, 0.11f);
            }
            else if (gardenManager.PlantIsDead(areaIndex, cell))
            {
                spriteRenderer.color = new Color(1f, 1f, 1f);
                Sprite deadSprite = Resources.Load<Sprite>("Flowers/DeadPlant/PLA_DeadPlant");
                spriteRenderer.sprite = deadSprite;
            }
            else
            {
                spriteRenderer.color = new Color(1f, 1f, 1f);
            }
        }

        public void UpdateVegetableSprite(Vector3Int cell)
        {
            Transform vegetableTransform = plantPrefabLookup[cell].transform;
            SpriteRenderer stalk = vegetableTransform.Find("Stalk").GetComponent<SpriteRenderer>();
            SpriteRenderer head = vegetableTransform.Find("Head").GetComponent<SpriteRenderer>();
            PlantState plant = gardenManager.GetPlant(areaIndex, cell);
            Genotype genotype = gardenManager.GetGenotype(areaIndex, cell);
            stalk.sprite = collection.GetSprite(plant.type, genotype, plant.growthStage);
            head.sprite = collection.GetVegetableHead(plant.type, genotype, plant.growthStage);
            vegetableTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * plant.genotype.SpriteSize();

            if (gardenManager.PlantIsWilted(areaIndex, cell))
            {
                stalk.color = new Color(0.71f, 0.4f, 0.11f);
                head.color = new Color(0.71f, 0.4f, 0.11f);
            }
            else if (gardenManager.PlantIsDead(areaIndex, cell))
            {
                stalk.color = new Color(1f, 1f, 1f);
                Sprite deadSprite = Resources.Load<Sprite>("Flowers/DeadPlant/PLA_DeadPlant");
                stalk.sprite = deadSprite;
            }
            else
            {
                stalk.color = new Color(1f, 1f, 1f);
                head.color = new Color(1f, 1f, 1f);
            }
        }

        public void UpdateFruitSprite(Vector3Int cell)
        {
            Transform fruitTransform = plantPrefabLookup[cell].transform;
            SpriteRenderer tree = fruitTransform.Find("Tree").GetComponent<SpriteRenderer>();
            SpriteRenderer fruit1 = fruitTransform.Find("Fruit1").GetComponent<SpriteRenderer>();
            SpriteRenderer fruit2 = fruitTransform.Find("Fruit2").GetComponent<SpriteRenderer>();
            SpriteRenderer fruit3 = fruitTransform.Find("Fruit3").GetComponent<SpriteRenderer>();

            PlantState plant = gardenManager.GetPlant(areaIndex, cell);
            Genotype genotype = gardenManager.GetGenotype(areaIndex, cell);

            tree.sprite = collection.GetFruitTree(plant.type, genotype, plant.growthStage);
            fruit1.sprite = collection.GetFruitFruit(plant.type, genotype, plant.growthStage);
            fruit2.sprite = collection.GetFruitFruit(plant.type, genotype, plant.growthStage);
            fruit3.sprite = collection.GetFruitFruit(plant.type, genotype, plant.growthStage);

            fruitTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * plant.genotype.SpriteSize();
        }

        public void UpdateSprite(Vector3Int cell)
        {
            PlantId id = gardenManager.GetPlantType(areaIndex, cell);
            if (collection.IsFlower(id))
            {
                UpdateFlowerSprite(cell);
            }
            else if (collection.IsVegetable(id))
            {
                UpdateVegetableSprite(cell);
            }
            else if (collection.IsFruit(id))
            {
                UpdateFruitSprite(cell);
            }
        }

        public void InstantiatePlantPrefab(Vector3Int cell, PlantId id, int growthStage)
        {
            Vector3 centerOfCell = tilemap.GetCellCenterWorld(cell);
            if (collection.IsFlower(id))
            {
                plantPrefabLookup[cell] = Instantiate(defaultFlowerPrefab, centerOfCell, Quaternion.identity);
            }
            else if (collection.IsVegetable(id))
            {
                plantPrefabLookup[cell] = Instantiate(defaultVegetablePrefab, centerOfCell, Quaternion.identity);
            }
            else if (collection.IsFruit(id))
            {
                plantPrefabLookup[cell] = Instantiate(defaultFruitPrefab, centerOfCell, Quaternion.identity);
            }
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
            // Get an initial plantList (with their original timers) and increment all waterTimers
            // in that list
            List<PlantState> plantsToUpdate = gardenManager.GetPlants(areaIndex);
            foreach (PlantState plant in plantsToUpdate)
            {
                gardenManager.IncrementWaterTimer(areaIndex, plant.cell, value);
            }

            // Due to the nature of GetPlants(), the "plantList" list actually does not return to us
            // updated waterTimers after an IncrementTimer. As such, I'm temporarily re-grabbing the list
            // as an updatedPlantList in order to get the newly updated waterTimers.

            // This will also fix issues with regards to lag whenever a plant enters wilt/death stage, as
            // well as issues regarding plants first appearing wilted/dead despite having been watered and
            // being in a healthy second stage
            List<PlantState> updatedPlantsList = gardenManager.GetPlants(areaIndex);
            foreach (PlantState updatedPlant in updatedPlantsList)
            {
                if (updatedPlant.waterStage == 1 && updatedPlant.waterTimer >= collection.GetPlant(updatedPlant.type).growthTime)
                {
                    // Debug.Log("Plant Growth via IncrementWaterTimer");
                    gardenManager.UpdateGrowthStage(areaIndex, updatedPlant.cell);

                    UpdateSprite(updatedPlant.cell);
                }
                else
                {
                    if (updatedPlant.waterTimer == collection.GetPlant(updatedPlant.type).growthTime)
                    {
                        Debug.Log("Plant is ready for water");
                    }
                    else if (updatedPlant.waterTimer == gardenManager.WiltTime
                        || (gardenManager.PlantIsWilted(areaIndex, updatedPlant.cell) && returnFromPause)) //>= 240)
                    {
                        // Wilted
                        wiltedPlantList.Add(updatedPlant);
                        UpdateSprite(updatedPlant.cell);

                        gardenVFX.PlayDryParticle(tilemap.GetCellCenterWorld(updatedPlant.cell));
                    }
                    else if (!gardenManager.PlantIsFullyGrown(areaIndex, updatedPlant.cell))
                    {
                        if (updatedPlant.waterTimer == gardenManager.DeathTime
                            || (gardenManager.PlantIsDead(areaIndex, updatedPlant.cell) && returnFromPause)) //>= 336)
                        {
                            // Dead
                            UpdateSprite(updatedPlant.cell);

                            gardenVFX.PlayDryParticle(tilemap.GetCellCenterWorld(updatedPlant.cell));
                        }
                    }
                }

                if (returnFromPause) UpdateSprite(updatedPlant.cell);
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
                
                foreach(PlantState plantToRemove in wiltedPlantList)
                {
                    if(plantToRemove.cell == cell) {
                        wiltedPlantList.Remove(plantToRemove);
                        break;
                    }
                }
                UpdateSprite(cell);
            }

            // Spawn a particle system so we know that a plant has been watered
            gardenVFX.PlayWaterParticle(tilemap.GetCellCenterWorld(cell));
        }

        public void FertilizePlant(Vector3Int cell)
        {
            if (gardenManager.SetFertilization(areaIndex, cell))
            {
                //Spawn a particle system so we know that a plant has been fertilized
                gardenVFX.PlayFertilizerParticle(tilemap.GetCellCenterWorld(cell));
            }
        }

        public List<PlantState> GetNeighbors(Vector3Int cell)
        {
            return gardenManager.GetNeighbors(areaIndex, cell);
        }

        public List<PlantState> GetBreedingCandidates(Vector3Int cell)
        {
            return gardenManager.GetBreedingCandidates(areaIndex, cell);
        }

        public bool HarvestPlant(Vector3Int cell)
        {
            if (gardenManager.IsEmpty(areaIndex, cell)) return false;

            int maxGrowthStages = collection.GetPlant(
		        gardenManager.GetPlantType(areaIndex, cell))
		        .growthStages;

            string debug = "Breeding Candidate Debug (Click to Expand)\n";

            if (gardenManager.PlantIsBreedable(areaIndex, cell))
            {
                debug += "Breeding candidates: ";
                List<PlantState> breedingCandidates = GetBreedingCandidates(cell);
                foreach (PlantState neighbor in breedingCandidates)
                {
                    debug += neighbor.genotype + " ";
                }

                PlantId motherPlantType = gardenManager.GetPlantType(areaIndex, cell);
                Genotype motherGenotype = gardenManager.GetGenotype(areaIndex, cell);
                PlantId childPlantType = motherPlantType;
                Genotype childGenotype = motherGenotype;

                if (breedingCandidates.Count != 0)
                {
                    Genotype fatherGenotype = breedingCandidates[Random.Range(0, breedingCandidates.Count)].genotype;
                    childGenotype = motherGenotype.Cross(fatherGenotype);
                    debug += "\nBreeding occured.";
                }
                else
                    debug += "\nNo breeding occured.";

                Debug.Log(debug);

                PlantProperties properties = collection.GetPlant(childPlantType);
                EventManager.instance.HandleEVENT_INVENTORY_ADD_PLANT((ushort)motherPlantType, motherGenotype);

                float trueDropRate = DropRate;
                if (GetPlant(cell).isFertilized) trueDropRate += FertilizationBonus;
                if (Random.Range(0f, 1f) <= trueDropRate/100)
                {
                    EventManager.instance.HandleEVENT_INVENTORY_ADD_SEED((ushort)childPlantType, childGenotype);
                }

                DestroyPlant(cell);

                CharacterId golemType;
                if (collection.PlantToGolem(motherPlantType, out golemType))
                    EventManager.instance.HandleEVENT_GOLEM_SPAWN((ushort)golemType, cell);
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

        [ContextMenu("ResetGarden")]
        public void ResetGarden()
        {
            Vector3Int playableTopLeft = new Vector3Int(-8, 4, 0);
            Vector3Int playableBottomRight = new Vector3Int(6, -5, 0);
            for (int x = -8; x <= 6; x++)
            {
		        for (int y = -5; y <= 4; y++)
                {
                    ChangeGardenTileToGrass(new Vector3Int(x, y, 0)); 
		        }
	        }
            ClearPlants();
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

        [ContextMenu("FlowerTest")]
        public void FlowerTest()
        {
            ResetGarden();
            Vector3Int topLeft = Vector3Int.zero + 3 * Vector3Int.up + 7 * Vector3Int.left;
            int row = 0;
            foreach (FlowerId flowerId in System.Enum.GetValues(typeof(FlowerId)))
            {
                Vector3Int leftTile = topLeft + row * Vector3Int.down;
                Vector3Int right = Vector3Int.right;
                for (int j = 0; j < 7; j++) {
                    ChangeGardenTileToPlot_Occupied(leftTile + j * right);
	            }
                PlantId plantId = (PlantId)flowerId;
                Genotype.Generation mega = Genotype.Generation.F2;
                CreatePlant(plantId, new Genotype("AaBb"), leftTile + right * 0, 0);
                CreatePlant(plantId, new Genotype("AaBb"), leftTile + right * 1, 1);
                CreatePlant(plantId, new Genotype("aabb"), leftTile + right * 2, 2);
                CreatePlant(plantId, new Genotype("AaBb"), leftTile + right * 3, 2);
                CreatePlant(plantId, new Genotype("AABB"), leftTile + right * 4, 2);
                CreatePlant(plantId, new Genotype("aabb", mega), leftTile + right * 5, 2);
                CreatePlant(plantId, new Genotype("AABB", mega), leftTile + right * 6, 2);
                row++;
	        }
        }

        [ContextMenu("VegetableTest")]
        public void VegetableTest()
        {
            ResetGarden();
            Vector3Int topLeft = Vector3Int.zero + 3 * Vector3Int.up + 7 * Vector3Int.left;
            int row = 0;
            foreach (VegetableId vegId in System.Enum.GetValues(typeof(VegetableId)))
            {
                Vector3Int leftTile = topLeft + row * Vector3Int.down;
                Vector3Int right = Vector3Int.right;
                for (int j = 0; j < 7; j++)
                {
                    ChangeGardenTileToPlot_Occupied(leftTile + j * right);
                }
                PlantId plantId = (PlantId)vegId;
                Genotype.Generation mega = Genotype.Generation.F2;
                CreatePlant(plantId, new Genotype("AaBb"), leftTile + right * 0, 0);
                CreatePlant(plantId, new Genotype("AaBb"), leftTile + right * 1, 1);
                CreatePlant(plantId, new Genotype("aabb"), leftTile + right * 2, 2);
                CreatePlant(plantId, new Genotype("AaBb"), leftTile + right * 3, 2);
                CreatePlant(plantId, new Genotype("AABB"), leftTile + right * 4, 2);
                CreatePlant(plantId, new Genotype("aabb", mega), leftTile + right * 5, 2);
                CreatePlant(plantId, new Genotype("AABB", mega), leftTile + right * 6, 2);
                row++;
            }
        }

        [ContextMenu("FruitTest")]
        public void FruitTest()
        {
            ResetGarden();
            Vector3Int topLeft = Vector3Int.zero + 3 * Vector3Int.up + 7 * Vector3Int.left;
            int row = 0;
            foreach (FruitId fruitId in System.Enum.GetValues(typeof(FruitId)))
            {
                Vector3Int leftTile = topLeft + row * Vector3Int.down;
                Vector3Int right = Vector3Int.right;
                for (int j = 0; j < 7; j++)
                {
                    ChangeGardenTileToPlot_Occupied(leftTile + j * right);
                }
                PlantId plantId = (PlantId)fruitId;
                Genotype.Generation mega = Genotype.Generation.F2;
                CreatePlant(plantId, new Genotype("AaBb"), leftTile + right * 0, 0);
                CreatePlant(plantId, new Genotype("AaBb"), leftTile + right * 1, 1);
                CreatePlant(plantId, new Genotype("aabb"), leftTile + right * 2, 2);
                CreatePlant(plantId, new Genotype("AaBb"), leftTile + right * 3, 2);
                CreatePlant(plantId, new Genotype("AABB"), leftTile + right * 4, 2);
                CreatePlant(plantId, new Genotype("aabb", mega), leftTile + right * 5, 2);
                CreatePlant(plantId, new Genotype("AABB", mega), leftTile + right * 6, 2);
                row++;
            }
        }

        [ContextMenu("GenotypeSpriteTest")]
        public void GenotypeSpriteTest()
        {
            ResetGarden();
            Vector3Int right = Vector3Int.zero + 3 * Vector3Int.up;
            for (int i = 0; i <= 8; i += 2)
            {
                DestroyPlant(right + i * Vector3Int.down);
                ChangeGardenTileToPlot_Occupied(right + i * Vector3Int.down);
            }
            CreatePlant(PlantId.Rose, new Genotype("aabb"), right + 0 * Vector3Int.down, 2);
            CreatePlant(PlantId.Rose, new Genotype("AaBb"), right + 2 * Vector3Int.down, 2);
            CreatePlant(PlantId.Rose, new Genotype("AABB"), right + 4 * Vector3Int.down, 2);
            // Megas
            CreatePlant(PlantId.Rose, new Genotype("Aabb", Genotype.Generation.F2), right + 6 * Vector3Int.down, 2);
            CreatePlant(PlantId.Rose, new Genotype("AaBB", Genotype.Generation.F2), right + 8 * Vector3Int.down, 2);
        }

        [ContextMenu("CrossBreedTest")]
        public void CrossBreedTest()
        {
            Genotype first;
            Genotype second;

            Debug.Log("child is p1");
            first = new Genotype("aaBb");
            second = new Genotype("aaBb");
            first.Cross(second);

            Debug.Log("if both parents are f2, child is f2");
            first = new Genotype("aaBb", Genotype.Generation.F2);
            second = new Genotype("aaBb", Genotype.Generation.F2);
            first.Cross(second);

            Debug.Log("if parents are duplicate homozygous, child is F1");
            first = new Genotype("aabb");
            second = new Genotype("aabb");
            first.Cross(second);

            first = new Genotype("aaBB");
            second = new Genotype("aaBB");
            first.Cross(second);

            Debug.Log("if parents are duplicate homozygous and both f1, child is f2");
            first = new Genotype("aabb", Genotype.Generation.F1);
            second = new Genotype("aabb", Genotype.Generation.F1);
            first.Cross(second);

            first = new Genotype("aaBB", Genotype.Generation.F1);
            second = new Genotype("aaBB", Genotype.Generation.F1);
            first.Cross(second);

            Debug.Log("if parents are duplicate homozygous and one is f1 and one is f2, child is f2");
            first = new Genotype("aabb", Genotype.Generation.F2);
            second = new Genotype("aabb", Genotype.Generation.F1);
            first.Cross(second);

            first = new Genotype("aaBB", Genotype.Generation.F2);
            second = new Genotype("aaBB", Genotype.Generation.F1);
            first.Cross(second);
        }

        [ContextMenu("IdTypeTest")]
        public void IdTypeTest()
        {
            Debug.Log("1001 is flower " + collection.IsFlower((PlantId)1001));
            Debug.Log("1018 is flower " + collection.IsFlower((PlantId)1018));
            Debug.Log("1008 is vegetable " + collection.IsVegetable((PlantId)1008));
            Debug.Log("1001 is vegetable " + collection.IsVegetable((PlantId)1001));
            Debug.Log("1015 is fruit " + collection.IsFruit((PlantId)1015));
            Debug.Log("1008 is fruit " + collection.IsFruit((PlantId)1008));
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
