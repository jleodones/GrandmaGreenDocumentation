using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.Tilemaps;

namespace GrandmaGreen
{
    /// <summary>
    /// TODO: Implement functionality for save/loading changed tiles 
    /// </summary>
    public class GardenAreaController : MonoBehaviour
    {
        [Header("Area References")]
        public GardenData gardenData;
        public Tilemap tilemap;
        public Pathfinder pathfinder;
        public Collider areaBounds;

        [Header("Prefab References")]
        public PlantInteractable plantInteractablePrefab;

        [SerializeField] Dictionary<PlantInteractable, PlantState> m_plantMap;
        [SerializeField] GameObject[] m_gardenPlants;

        public event System.Action<Vector2Int> onGardenSelection;
        [field: SerializeField] public GameObject currentSelection { get; private set; }

        void Start()
        {
            pathfinder.LoadGrid();
        }

        [ContextMenu("Intialize")]
        void InitializeGarden()
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
        public void BakeNavGrid()
        {
            NavGridBaker.Bake(
                pathfinder.gridData,
                pathfinder.settings.obstacleMask,
                pathfinder.settings.agentRadius,
                pathfinder.settings.occupiedTileWeight);
        }

        public void GardenSelection(Vector3 worldPos)
        {
            Vector2Int gridPos = gardenData.WorldToGrid(worldPos);

            int index = gridPos.x + gridPos.y * gardenData.gridSize.x;
            currentSelection = m_gardenPlants[index];

            if (currentSelection != null)
            {
                Debug.Log(m_gardenPlants[index].name + "selected");
            }

            onGardenSelection?.Invoke(gridPos);
        }

        public void PlantInteraction()
        {
            if (currentSelection == null)
                return;



            currentSelection = null;
        }

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

            //  traitList = new ITraitSetData[phenotypeData.TraitCount];

            for (int i = 0; i < phenotypeData.TraitCount; i++)
            {
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