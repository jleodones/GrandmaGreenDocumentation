using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using Core.Input;
using Pathfinding;
using GrandmaGreen;
using GrandmaGreen.Garden;

public class Tool : MonoBehaviour
{
    private int currIndex = 0;

    [Header("Garden Management")]
    public Tilemap gardenMap;

    [SerializeField]
    private List<TileType> tileTypesList;
    private Dictionary<TileBase, TileType> dataFromTiles;

    [SerializeField] 
    private GameObject GardenManagement;

    [Header("Temporary Info - Prototyping")]
    public PlantType plantType;

    [Header("Tools Management")]
    [SerializeField] private ToolTypeDictionary toolType;
    [SerializeField] private SplineFollow script;

    private void Start()
    {
        //Temporary debug interactions for testing
        PlayerTool.interactInput += TileInteract;
        PlayerTool.swapInput += SwapTools;

        //Storing all possible TileTypes and their attributes to use for later
        dataFromTiles = new Dictionary<TileBase, TileType>();

        foreach(var tileType in tileTypesList)
        {
            dataFromTiles.Add(tileType.tile, tileType);
        }
    }

    private void TileInteract()
    {
        // Grandma's Position -> Tile Position
        Vector3 grandmaPos = transform.position;
        Vector3Int grandmaGridPos = gardenMap.WorldToCell(grandmaPos);
        TileBase tileToInteract = gardenMap.GetTile(grandmaGridPos);

        if(toolType[currIndex].toolName == "Trowel")
        {
            TrowelInteract(tileToInteract, grandmaGridPos);

        } else if(toolType[currIndex].toolName == "Seeds")
        {
            SeedInteract(tileToInteract, grandmaGridPos);

        } else if(toolType[currIndex].toolName == "Scissors")
        {
            ScissorInteract();
            
        }
    }

    private void TrowelInteract(TileBase currTile, Vector3Int currPos)
    {
        if(!script.isFollowing){

            if(dataFromTiles[currTile].isPlottable)
            {
                gardenMap.SetTile(currPos, tileTypesList[1].tile);

            } else if(dataFromTiles[currTile].isPlantable || dataFromTiles[currTile].isOccupied)
            {
                // "Plot" and "Occupied Plot" Tile -> Grass Tile
                // Sending data to Plants to destroy seeds on specific tiles
                if(dataFromTiles[currTile].isOccupied)
                {
                    GardenManagement.GetComponent<GardenAreaController>().TrowelAtPos(currPos);
                }
                gardenMap.SetTile(currPos, tileTypesList[0].tile);
            }
            
        } 
    }

    private void SeedInteract(TileBase currTile, Vector3Int currPos)
    {
        //Placing the Plant Prefab on a tile and setting the Tile to "Occupied Plot Tile"
        if(dataFromTiles[currTile].isPlantable)
        {
            Vector3 halfwayPos = currPos + (Vector3.up/4) + (Vector3.right/2);
            GardenManagement.GetComponent<GardenAreaController>().PlacePlantPrefab(plantType, halfwayPos, 0);
            gardenMap.SetTile(currPos, tileTypesList[2].tile);
        }
    }

    private void ScissorInteract()
    {
        EventManager.instance.HandleEVENT_GOLEM_SPAWN(transform.parent.gameObject.GetInstanceID());
    }

    private void SwapTools()
    {
        currIndex++;
        if(currIndex > toolType.toolData.Count - 1)
        {
            currIndex = 0;
        }
    }
}
