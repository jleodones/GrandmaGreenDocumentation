using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GrandmaGreen.Entities;
using GrandmaGreen.Collections;
using NaughtyAttributes;
using UnityEngine.Tilemaps;
using SpookuleleAudio;

namespace GrandmaGreen.Garden
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Garden/Player Tool Data")]
    public class PlayerToolData : ScriptableObject
    {
        public EntityController playerController;
        public GardenToolSet toolSet;
        public ToolData currentTool;
        public CollectionsSO collection;

        public ASoundContainer toolSelectionStartSFX;
        public ASoundContainer toolSelectionEndSFX;

        public event System.Action onToolSelectionStart;
        public event System.Action onToolSelectionEnd;
        public event System.Action<ToolData> onToolSelected;

        public ToolActionData lastToolAction;

        public bool toolSelectionActive = false;
        
        public PlantId equippedSeed;
        public Genotype equippedSeedGenotype;

        public void ToggleToolSelection()
        {
            if (!toolSelectionActive)
                StartToolSelection();
            else
                EndToolSelection();
        }

        public void StartToolSelection()
        {
            toolSelectionStartSFX?.Play();

            if (playerController.entity.isPathing)
                playerController.CancelDestination();

            toolSelectionActive = true;
            onToolSelectionStart?.Invoke();
        }

        public void EndToolSelection()
        {
            if (!toolSelectionActive)
                return;

            toolSelectionEndSFX?.Play();

            if (playerController.entity.isPathing)
                playerController.CancelDestination();

            toolSelectionActive = false;
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
            tool.selectedSFX?.Play();

            currentTool = tool;
            onToolSelected?.Invoke(currentTool);
            if (currentTool.toolIndex != 3)
            {
                equippedSeed = 0;
            }
            EndToolSelection();
        }

        public void SetEquippedSeed(ushort plantIndex, Genotype genotype)
        {
            equippedSeed = (PlantId) plantIndex;
            equippedSeedGenotype = genotype;
        }

        public void SetToolAction(TileData tile, Vector3Int cell, GardenAreaController area)
        {

            lastToolAction = new ToolActionData()
            {
                tool = currentTool,
                tile = tile,
                gridcell = cell,
                area = area,
                seedType = equippedSeed,
                seedGenotype = equippedSeedGenotype
            };
        }

        public void DoCurrentToolAction()
        {
            toolSet.ToolAction(lastToolAction);
        }
    }
}
