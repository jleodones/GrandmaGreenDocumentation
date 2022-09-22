using Core.Input;
using NaughtyAttributes;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace GrandmaGreen.Garden
{
    /// <summary>
    /// Responsible for functionality of a single Garden screen
    /// TODO: Implement functionality for save/loading changed tiles 
    /// </summary>
    public class GardenAreaController : MonoBehaviour
    {
        [Header("Area References")]
        public GardenAreaServicer areaServicer;
        public GardenData gardenData;
        public Tilemap tilemap;
        public Pathfinder pathfinder;
        public Collider areaBounds;

        [Header("Plant Management")]
        public PlantTypeDictionary plantTypeDictionary;
        public PlantStateManager plantStateManager;
        public GameObject[,] plantPrefabs;

        [Header("Prefab References")]
        public PlantInteractable plantInteractablePrefab;

        [Header("Area Variables")]
        [SerializeField] int areaIndex = 0;
        [SerializeField][ReadOnly] bool areaActive;
        [field: SerializeField][field: ReadOnly] public GameObject currentSelection { get; private set; }

        [SerializeField][ReadOnly] Dictionary<PlantInteractable, PlantState> m_plantMap;
        [SerializeField][ReadOnly] GameObject[] m_gardenPlants;

        public event System.Action<Vector2Int> onGardenSelection;
        public event System.Action onActivation;
        public event System.Action onDeactivation;

        void Awake()
        {
            areaServicer.RegisterAreaController(this, areaIndex);
            plantStateManager.RegisterGarden(tilemap.size, areaIndex);
            areaActive = false;
        }

        void Start()
        {
            if (SceneManager.GetActiveScene().name != "TestGardenTileData")
            {
                InitializeGarden();
            }

            plantTypeDictionary.LoadPlantTypes();
        }

        public void Activate()
        {
            pathfinder.LoadGrid();
            plantPrefabs = new GameObject[tilemap.size.x, tilemap.size.y];
            areaActive = true;

            onActivation?.Invoke();
        }

        public void Deactivate()
        {
            pathfinder.ReleaseGrid();
            areaActive = false;

            onDeactivation?.Invoke();
        }

        public void TrowelAtPos(Vector3 pos)
        {
            Debug.Log(string.Format("World pos: {0}\nTile: {1}",
                pos, gardenData.WorldToGrid(pos)));
        }

        public void PlacePlantPrefab(PlantType type, Vector3 pos, int growthStage)
        {
            Vector3Int cell = tilemap.WorldToCell(pos);
            Vector3Int cellToArr = cell - tilemap.origin;
            plantPrefabs[cellToArr.x, cellToArr.y] = Instantiate(type.growthStagePrefabs[growthStage], pos, Quaternion.identity);
            plantStateManager.CreatePlant(type, areaIndex, cellToArr.x, cellToArr.y);
        }

        public void PlacePlantPrefabAtCell(PlantType type, Vector3Int cell, int growthStage)
        {
            Vector3 pos = tilemap.GetCellCenterWorld(cell);
            Vector3Int cellToArr = cell - tilemap.origin;
            plantPrefabs[cellToArr.x, cellToArr.y] = Instantiate(type.growthStagePrefabs[growthStage], pos, Quaternion.identity);
            plantStateManager.CreatePlant(type, areaIndex, cellToArr.x, cellToArr.y);
        }

        public void CreatePlantOnCell(PlantType type, Vector3Int cell)
        {
            Vector3 centerOfCell = tilemap.GetCellCenterWorld(cell);
            Vector3Int cellToArr = cell - tilemap.origin;
            plantPrefabs[cellToArr.x, cellToArr.y] = Instantiate(type.growthStagePrefabs[0], centerOfCell, Quaternion.identity);
            plantStateManager.CreatePlant(type, areaIndex, cellToArr.x, cellToArr.y);
        }

        public void DestroyPlantOnCell(Vector3Int cell)
        {
            Vector3Int cellToArr = cell - tilemap.origin;
            if (plantStateManager.IsEmpty(areaIndex, cellToArr.x, cellToArr.y))
            {
                Debug.Log(string.Format("No plant at {0}", cell));
            }
            else
            {
                Destroy(plantPrefabs[cellToArr.x, cellToArr.y]);
                plantStateManager.DestroyPlant(areaIndex, cellToArr.x, cellToArr.y);
            }
        }

        [ContextMenu("PlaceRoseBottomLeft")]
        public void PlaceRoseBottomLeft()
        {
            CreatePlantOnCell(plantTypeDictionary["Rose"], tilemap.origin);
        }

        [ContextMenu("DestroyRoseBottomLeft")]
        public void DestroyRoseBottomLeft()
        {
            DestroyPlantOnCell(tilemap.origin);
        }

        [ContextMenu("PlaceAllPlantTypePrefabs")]
        public void PlaceAllPlantTypePrefabs()
        {
            for (int i = 0; i < plantTypeDictionary.plantTypes.Length; i++)
            {
                PlantType plant = plantTypeDictionary.plantTypes[i];
                for (int j = 0; j < plant.growthStagePrefabs.Length; j++)
                {
                    PlacePlantPrefabAtCell(plant, tilemap.origin + i * Vector3Int.up + j * Vector3Int.right, j);
                }
            }
        }

        [ContextMenu("DestroyAllPlantPrefabs")]
        public void DestroyAllPlantPrefabs()
        {
            for (int i = 0; i < plantTypeDictionary.plantTypes.Length; i++)
            {
                PlantType plant = plantTypeDictionary.plantTypes[i];
                for (int j = 0; j < plant.growthStagePrefabs.Length; j++)
                {
                    DestroyPlantOnCell(tilemap.origin + i * Vector3Int.up + j * Vector3Int.right);
                }
            }
        }

        [ContextMenu("IntializeGarden")]
        /// <summary>
        /// Initalizes Garden including instantiating plants
        /// </summary>
        public void InitializeGarden()
        {
            m_gardenPlants = new GameObject[gardenData.gridSize.x * gardenData.gridSize.y];
            m_plantMap = new Dictionary<PlantInteractable, PlantState>(gardenData.plantStates.Count);

            PlantInteractable gardenPlant;

            for (int i = 0; i < gardenData.plantStates.Count; i++)
            {
                gardenPlant = Instantiate(plantInteractablePrefab, gardenData.IndexToWorldPos(gardenData.plantStates[i].gridIndex), Quaternion.identity);

                m_gardenPlants[gardenData.plantStates[i].gridIndex] = gardenPlant.gameObject;
                m_plantMap.Add(gardenPlant, gardenData.plantStates[i]);
            }
        }

        [ContextMenu("ParseTilemap")]
        /// <summary>
        /// Parses the world tilemap to cached grid nodes 
        /// Converts grid nodes into higher resolution nav grid
        /// </summary>
        public void ParseTilemap()
        {
            TilemapGridParser.ParseTilemap(
                tilemap,
                areaBounds.bounds
            );

            gardenData.gridSize = (Vector2Int)TilemapGridParser.tileSetSize;
            gardenData.cellSize = tilemap.cellSize;
            gardenData.worldOrigin = tilemap.CellToWorld(TilemapGridParser.tileSetOrigin) + (Vector3)gardenData.cellSize / 2;

            TilemapGridParser.ConvertToNavGrid(
                tilemap,
                pathfinder.gridData,
                pathfinder.settings);
        }

        [ContextMenu("BakeNavGrid")]
        /// <summary>
        /// Checks agaisnt obstacle colliders to add unpathable nodes
        /// </summary>
        public void BakeNavGrid()
        {
            NavGridBaker.Bake(
                pathfinder.gridData,
                pathfinder.settings.obstacleMask,
                pathfinder.settings.agentRadius,
                pathfinder.settings.occupiedTileWeight);
        }

        //Selects a garden tile in world space
        //Called through a Unity event as of now
        public void GardenSelection(Vector3 worldPos)
        {
            Vector2Int gridPos = gardenData.WorldToGrid(worldPos);

            int index = gridPos.x + gridPos.y * gardenData.gridSize.x;
            currentSelection = m_gardenPlants[index];

            if (currentSelection != null)
            {
                Debug.Log(currentSelection.name + "selected");
            }

            onGardenSelection?.Invoke(gridPos);
        }

        /// <summary>
        /// NOT USED FOR NOW
        /// </summary>
        public void PlantInteraction()
        {
            if (currentSelection == null)
                return;


            currentSelection = null;
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