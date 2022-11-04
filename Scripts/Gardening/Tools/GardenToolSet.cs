using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GrandmaGreen.Collections;

namespace GrandmaGreen.Garden
{
    public struct ToolActionData
    {
        public ToolData tool;
        public TileData tile;
        public Vector3Int gridcell;
        public GardenAreaController area;
        public SeedId seedType;
    }


    [CreateAssetMenu(menuName = "GrandmaGreen/Tools/GardenToolSet")]
    public class GardenToolSet : ScriptableObject
    {
        [SerializeField] List<ToolData> toolSet;

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

            switch(action.tool.toolIndex)
            {
                case 0:
                    break;
                case 1:
                    TrowelAction(action);
                    break;
                case 2:
                    FertilizerAction(action);
                    break;
                case 3:
                    SeedPacketAction(action);
                    break;
                case 4:
                    WateringAction(action);
                    break;
            }
        }

        void TrowelAction(ToolActionData action)
        {
            if (action.tile.plottable)
            {
                action.area.ChangeGardenTileToPlot_Empty(action.gridcell);
                action.tool.toolSFX[0].Play();

            }
            else if (action.tile.plantable)
            {
                // "Plot" Tile -> Grass Tile
                action.area.ChangeGardenTileToGrass(action.gridcell);
                action.tool.toolSFX[1].Play();
            }
            else if (action.tile.occupied)
            {
                if (action.area.HarvestPlantOnCell(action.gridcell))
                    action.tool.toolSFX[2].Play();

                action.area.ChangeGardenTileToPlot_Empty(action.gridcell);
            }
        }

        void FertilizerAction(ToolActionData action)
        {
            if(action.tile.occupied)
            {
                action.area.FertilizePlant(action.gridcell);
            }
        }

        void SeedPacketAction(ToolActionData action)
        {
            // Placing the Plant Prefab on a tile and setting the Tile to "Occupied Plot Tile"
            if (action.tile.plantable && action.seedType != 0)
            {
                action.area.CreatePlant(action.seedType, action.gridcell); 
                action.area.ChangeGardenTileToPlot_Occupied(action.gridcell);
                action.tool.toolSFX[0].Play();
            }
        }

        void WateringAction(ToolActionData action)
        {
            // Checking Tile for watering
            if (action.tile.occupied)
            {
                action.area.WaterPlant(action.gridcell);
            }
        }

    }
}