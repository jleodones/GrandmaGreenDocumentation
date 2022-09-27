using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GrandmaGreen.Garden
{
    public struct ToolActionData
    {
        public ToolData tool;
        public TileBase tile;
        public Vector3Int gridcell;
        public GardenAreaController area;
        public PlantType plantType;
    }


    [CreateAssetMenu(menuName = "GrandmaGreen/Tools/GardenToolSet")]
    public class GardenToolSet : ScriptableObject
    {
        [SerializeField] List<ToolData> toolSet;
        public TileStore tileStore;

        public ToolData this[int i]
        {
            get { return toolSet[i]; }
            set { toolSet[i] = value; }
        }
        public void ToolAction(ToolActionData action)
        {
            if (!toolSet.Contains(action.tool))
            {
                return;
            }
            
            if (action.tool.toolIndex == 1)
            {
                TrowelAction(action);
            }
            else if (action.tool.toolIndex == 3)
            {
                SeedPacketAction(action);
            }
        }

        void TrowelAction(ToolActionData action)
        {
            if (tileStore[action.tile].plottable)
            {
                action.area.tilemap.SetTile(action.gridcell, tileStore[1].tile);
                action.tool.toolSFX[0].Play();

            }
            else if (tileStore[action.tile].plantable)
            {
                // "Plot" Tile -> Grass Tile
                action.area.tilemap.SetTile(action.gridcell, tileStore[0].tile);
                action.tool.toolSFX[1].Play();
            }
            else if (tileStore[action.tile].occupied)
            {
                action.area.HarvestPlantOnCell(action.gridcell);
                action.area.tilemap.SetTile(action.gridcell, tileStore[1].tile);
                action.tool.toolSFX[2].Play();
            }
        }

        void SeedPacketAction(ToolActionData action)
        {

            //Placing the Plant Prefab on a tile and setting the Tile to "Occupied Plot Tile"
            if (tileStore[action.tile].plantable && action.plantType != default)
            {
                action.area.CreatePlantOnCell(action.plantType, action.gridcell);
                action.area.tilemap.SetTile(action.gridcell, tileStore[2].tile);
                action.tool.toolSFX[0].Play();
            }
        }

    }
}