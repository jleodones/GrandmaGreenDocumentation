using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GrandmaGreen.Entities;
using NaughtyAttributes;
using UnityEngine.Tilemaps;

namespace GrandmaGreen.Garden
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Garden/Player Tool Data")]
    public class PlayerToolData : ScriptableObject
    {
        public EntityController playerController;
        public GardenToolSet toolSet;
        public ToolData currentTool;
        public PlantTypeDictionary plantLookup;

        public event System.Action onToolSelectionStart;
        public event System.Action onToolSelectionEnd;
        public event System.Action<ToolData> onToolSelected;

        public ToolActionData lastToolAction;

        public bool toolSelectionActive = false;
        public int equippedPlant = -1;

        public void ToggleToolSelection()
        {
            if (!toolSelectionActive)
                StartToolSelection();
            else
                EndToolSelection();
        }

        public void StartToolSelection()
        {
            if (playerController.entity.isPathing)
                playerController.InterruptMovement();

            toolSelectionActive = true;
            onToolSelectionStart?.Invoke();
        }

        public void EndToolSelection()
        {
            if (!toolSelectionActive)
                return;

            if (playerController.entity.isPathing)
                playerController.InterruptMovement();

            toolSelectionActive = false;
            onToolSelectionEnd?.Invoke();
        }

        public void EmptySelection()
        {
            if (toolSelectionActive)
                ToolSelection(0);
        }

        public void ToolSelection(int index)
        {
            ToolSelection(toolSet[index]);
        }

        public void ToolSelection(ToolData tool)
        {
            currentTool = tool;
            onToolSelected?.Invoke(currentTool);
            if (currentTool.toolIndex != 3)
            {
                equippedPlant = -1;
            }
            // EndToolSelection();
        }

        public void SetEquippedPlant(int plantIndex) => equippedPlant = plantIndex;

        public void SetToolAction(TileBase tile, Vector3Int cell, GardenAreaController area)
        {
            PlantType plant = equippedPlant != -1 ? plantLookup[equippedPlant] : default;

            lastToolAction = new ToolActionData()
            {
                tool = currentTool,
                tile = tile,
                gridcell = cell,
                area = area,
                plantType = plant
            };
        }

        public void DoCurrentToolAction()
        {
            toolSet.ToolAction(lastToolAction);
        }
    }
}
