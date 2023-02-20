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
        public PlantId seedType;
        public Genotype seedGenotype;
    }


    [CreateAssetMenu(menuName = "GrandmaGreen/Tools/GardenToolSet")]
    public class GardenToolSet : ScriptableObject
    {
        [SerializeField] List<ToolData> toolSet;

        public event System.Action onTill;
        public event System.Action onPlant;
        public event System.Action onWater;
        public event System.Action onHarvest;

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

            switch (action.tool.toolIndex)
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

                onTill?.Invoke();

            }
            else if (action.tile.plantable)
            {
                // "Plot" Tile -> Grass Tile
                action.area.ChangeGardenTileToGrass(action.gridcell);
                action.tool.toolSFX[1].Play();
            }
            else if (action.tile.occupied)
            {
                if (action.area.HarvestPlant(action.gridcell))
                {
                    onHarvest?.Invoke();
                    action.tool.toolSFX[2].Play();
                }
                else
                    action.tool.toolSFX[3].Play();

                action.area.ChangeGardenTileToPlot_Empty(action.gridcell);
            }
        }

        void FertilizerAction(ToolActionData action)
        {
            if (!action.tile.fertilized)
            {
                if (action.tile.occupied)
                {
                    Debug.Log("Fertilizing an already occupied tile");
                    action.area.ChangeOccupiedGardenTileTo_Fertilized(action.gridcell);
                    action.area.FertilizePlant(action.gridcell);
                } else if(action.tile.plantable)
                {
                    Debug.Log("Fertilizing an unoccupied tile");
                    action.area.ChangeGardenTileToPlot_Fertilized(action.gridcell);
                }

                action.tool.toolSFX[0].Play();
            }
        }

        void SeedPacketAction(ToolActionData action)
        {
            // Placing the Plant Prefab on a tile and setting the Tile to "Occupied Plot Tile"
            if (action.tile.plantable && action.seedType != 0)
            {
                EventManager.instance.HandleEVENT_INVENTORY_REMOVE_SEED((ushort)action.seedType, action.seedGenotype);
                action.area.CreatePlant(action.seedType, action.seedGenotype, action.gridcell);

                if(!action.tile.fertilized)
                {
                    action.area.ChangeGardenTileToPlot_Occupied(action.gridcell);
                }
                else
                {
                    action.area.ChangeOccupiedGardenTileTo_Fertilized(action.gridcell);
                    action.area.FertilizePlant(action.gridcell);
                }

                action.tool.toolSFX[0].Play();

                onPlant?.Invoke();
            }
        }

        void WateringAction(ToolActionData action)
        {
            // Checking Tile for watering
            if (action.tile.occupied)
            {
                action.area.WaterPlant(action.gridcell);

                onWater?.Invoke();
            }

            action.tool.toolSFX[0].Play();
        }

    }
}