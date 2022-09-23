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
    private int currIndex = 0;

    [Header("Garden Management")]
    public Tilemap gardenMap;

    [SerializeField]
    private List<TileType> tileTypesList;
    [SerializeField] private Dictionary<TileBase, TileType> dataFromTiles;

    [SerializeField]
    private GardenAreaController GardenManagement;

    [Header("Temporary Info - Prototyping")]
    public PlantType plantType;

    [Header("Tools Management")]
    [SerializeField] private EntityController gardener;
    [SerializeField] private ToolTypeDictionary toolType;

    private void Start()
    {

        dataFromTiles = new Dictionary<TileBase, TileType>();

        foreach (var tileType in tileTypesList)
        {
            dataFromTiles.Add(tileType.tile, tileType);
        }
        GardenManagement.onTilemapSelection += TileInteract;

    }

    public Vector3Int selectedTilePos;
    public TileBase tileToInteract;
    private void TileInteract(Vector3Int gridSelection)
    {
        // Grandma's Position -> Tile Position

        selectedTilePos = gridSelection;
        tileToInteract = gardenMap.GetTile(selectedTilePos);

        gardener.entity.splineFollow.onComplete += DoInteraction;
    }

    void DoInteraction()
    {
        Debug.Log("Do interaction");
        gardener.entity.splineFollow.onComplete -= DoInteraction;
        
        if (toolType[currIndex].toolName == "Trowel")
        {
            TrowelInteract(tileToInteract, selectedTilePos);

        }
        else if (toolType[currIndex].toolName == "Seeds")
        {
            SeedInteract(tileToInteract, selectedTilePos);

        }
        else if (toolType[currIndex].toolName == "Scissors")
        {
            ScissorInteract();

        }
        
    }

    private void TrowelInteract(TileBase currTile, Vector3Int cell)
    {
        if (dataFromTiles[currTile].isPlottable)
        {
            Debug.Log(cell);
            gardenMap.SetTile(cell, tileTypesList[1].tile);
            gardenMap.RefreshTile(cell);

        }
        else if (dataFromTiles[currTile].isPlantable || dataFromTiles[currTile].isOccupied)
        {
            // "Plot" and "Occupied Plot" Tile -> Grass Tile
            // Sending data to Plants to destroy seeds on specific tiles
            if (dataFromTiles[currTile].isOccupied)
            {
                GardenManagement.HarvestPlantOnCell(cell);
            }
            gardenMap.SetTile(cell, tileTypesList[0].tile);
        }


    }

    private void SeedInteract(TileBase currTile, Vector3Int cell)
    {
        //Placing the Plant Prefab on a tile and setting the Tile to "Occupied Plot Tile"
        if (dataFromTiles[currTile].isPlantable)
        {
            Vector3 halfwayPos = cell + (Vector3.up / 4) + (Vector3.right / 2);
            GardenManagement.CreatePlantOnCell(plantType, cell);
            gardenMap.SetTile(cell, tileTypesList[2].tile);
        }
    }

    private void ScissorInteract()
    {
        //EventManager.instance.HandleEVENT_GOLEM_SPAWN(transform.parent.gameObject.GetInstanceID());
    }

    private void SwapTools()
    {
        currIndex++;
        if (currIndex > toolType.toolData.Count - 1)
        {
            currIndex = 0;
        }
    }
}
