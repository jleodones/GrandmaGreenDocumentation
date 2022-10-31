using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;
using NaughtyAttributes;
using GrandmaGreen.Entities;

namespace GrandmaGreen
{
    public class AreaController : MonoBehaviour
    {
        [Header("Area References")]

        public AreaServices areaServicer;
        public Tilemap tilemap;
        public Pathfinder pathfinder;
        public Collider areaBounds;
        public EntityController playerController;

        [field: Header("Area Variables")]
        [field: SerializeField] public int areaIndex { get; protected set; } = 0;
        [field: SerializeField][field: ReadOnly] public Vector3Int lastSelectedCell { get; protected set; }
        [field: SerializeField][field: ReadOnly] public TileBase lastSelectedTile { get; protected set; }


        public event System.Action<Vector3Int> onTilemapSelection;
        public event System.Action onActivation;
        public event System.Action onDeactivation;

        public virtual void Awake()
        {
            areaServicer.RegisterAreaController(this);
        }

        public virtual void Activate()
        {
            pathfinder.LoadGrid();
            onActivation?.Invoke();
        }

        public virtual void Deactivate()
        {
            onDeactivation?.Invoke();
        }

        void OnDestroy()
        {
            areaServicer.DeactivateAreaController(areaIndex);
            areaServicer.DesregisterAreaController(this);
        }

        public virtual void AreaSelection(Vector3 worldPoint)
        {
            lastSelectedCell = tilemap.WorldToCell(worldPoint);
            lastSelectedTile = tilemap.GetTile(lastSelectedCell);
            onTilemapSelection?.Invoke(lastSelectedCell);

            playerController.SetDestination(worldPoint);

            if (((lastSelectedTile as IGameTile)) != null)
            {
                playerController.QueueEntityAction(((IGameTile)lastSelectedTile).DoTileAction);
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


    }
}
