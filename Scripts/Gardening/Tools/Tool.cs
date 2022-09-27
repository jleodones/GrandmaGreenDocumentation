using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using Core.Input;
using Pathfinding;
using GrandmaGreen;
using GrandmaGreen.Garden;
using GrandmaGreen.Entities;

public class Tool : MonoBehaviour
{
    [Header("Garden Management")]
    public Tilemap gardenMap;

    [SerializeField]
    private List<TileType> tileTypesList;
    private Dictionary<TileBase, TileType> dataFromTiles;

    [SerializeField]
    private GardenAreaController GardenManagement;

    [Header("Tools Management")]
    [SerializeField] private EntityController gardener;
    [SerializeField] private GardenToolSet toolType;


    [Header("Temporary Info - Prototyping")]
    public PlantType plantType;
    public int currIndex = 0;

    private Vector3Int selectedTilePos;
    private TileBase tileToInteract;

    void Start()
    {
        dataFromTiles = new Dictionary<TileBase, TileType>();

        foreach (var tileType in tileTypesList)
        {
            dataFromTiles.Add(tileType.tile, tileType);
        }

    }

    void OnEnable()
    {
        GardenManagement.onTilemapSelection += TileInteract;

    }

    void OnDisable()
    {
        GardenManagement.onTilemapSelection -= TileInteract;

    }

    void TileInteract(Vector3Int gridSelection)
    {
        // Grandma's Position -> Tile Position
        selectedTilePos = gridSelection;
        tileToInteract = gardenMap.GetTile(selectedTilePos);

        gardener.entity.splineFollow.onComplete += DoInteraction;
    }

    void DoInteraction()
    {
        //Debug.Log("Do interaction");
        gardener.entity.splineFollow.onComplete -= DoInteraction;

        if (currIndex == 1)
        {
            TrowelInteract(tileToInteract, selectedTilePos);
        }
        else if (currIndex == 2)
        {
            SeedInteract(tileToInteract, selectedTilePos);
        }
    }

    public void ToolSwap(string toolName)
    {
        if (toolName == "trowel")
        {
            currIndex = 1;
        }
        else if (toolName == "seeds")
        {
            currIndex = 2;
        }
        else
        {
            currIndex = 0;
        }
    }

    void TrowelInteract(TileBase currTile, Vector3Int cell)
    {
        if (dataFromTiles[currTile].isPlottable)
        {
            //Debug.Log(cell);
            gardenMap.SetTile(cell, tileTypesList[1].tile);
            //gardenMap.RefreshTile(cell);
        }
        else if (dataFromTiles[currTile].isPlantable)
        {
            // "Plot" Tile -> Grass Tile
            gardenMap.SetTile(cell, tileTypesList[0].tile);
        }
        else if (dataFromTiles[currTile].isOccupied)
        {
            GardenManagement.HarvestPlantOnCell(cell);
            gardenMap.SetTile(cell, tileTypesList[1].tile);
        }
    }

    void SeedInteract(TileBase currTile, Vector3Int cell)
    {
        //Placing the Plant Prefab on a tile and setting the Tile to "Occupied Plot Tile"
        if (dataFromTiles[currTile].isPlantable)
        {
            GardenManagement.CreatePlantOnCell(plantType, cell);
            gardenMap.SetTile(cell, tileTypesList[2].tile);
        }
    }
}
