using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;
using NaughtyAttributes;
using GrandmaGreen.Entities;
using Core.Input;


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
        public TileStore tileStore;
        public Transform[] enterencePoints = new Transform[4];

        [field: Header("Area Variables")]
        [field: SerializeField] public int areaIndex { get; protected set; } = 0;
        [field: SerializeField][field: ReadOnly] public Vector3 lastSelectedPosition { get; protected set; }
        [field: SerializeField][field: ReadOnly] public Vector3 lastDraggedPosition { get; protected set; }
        [field: SerializeField][field: ReadOnly] public Vector3Int lastSelectedCell { get; protected set; }
        [field: SerializeField][field: ReadOnly] public TileBase lastSelectedTile { get; protected set; }


        public event System.Action<Vector3Int> onTilemapSelection;
        public event System.Action onActivation;
        public event System.Action onDeactivation;

        public virtual void Awake()
        {
            areaServicer.RegisterAreaController(this);
        }

        public virtual void OnValidate()
        {
            if (enterencePoints.Length < 4)
                enterencePoints = new Transform[4];
        }

        public virtual void Activate()
        {
            pathfinder.LoadGrid();
            onActivation?.Invoke();

            int enterenceIndex = (int)(areaServicer.exitState.exitSide + 2);
            if (enterenceIndex > 3) enterenceIndex -= 4;

            playerController.entity.transform.position = enterencePoints[enterenceIndex].position;
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

        public virtual void ProcessAreaInput(InteractionEventData eventData)
        {
            if (eventData.interactionState.phase == PointerState.Phase.DOWN)
            {
                AreaSelection(eventData.interactionPoint);
            }
            else if (eventData.interactionState.phase == PointerState.Phase.DRAG)
            {
                AreaDragged(eventData.interactionPoint);
            }
        }


        public virtual void AreaSelection(Vector3 worldPoint)
        {
            lastSelectedPosition = worldPoint;
            lastSelectedCell = tilemap.WorldToCell(worldPoint);




            lastSelectedTile = tilemap.GetTile(lastSelectedCell);

            playerController.ClearActionQueue();
            onTilemapSelection?.Invoke(lastSelectedCell);

            playerController.SetDestination(CheckPlayerDestination(worldPoint));
            // Release golem selected
            EventManager.instance.HandleEVENT_GOLEM_RELEASE_SELECTED();


            if (((lastSelectedTile as IGameTile)) != null)
            {
                playerController.QueueEntityAction(((IGameTile)lastSelectedTile).DoTileAction);

            }
        }

        Vector3 CheckPlayerDestination(Vector3 worldPoint)
        {
            Vector2 direction = (worldPoint - playerController.GetEntityPos()).normalized;
            Vector2Int offset;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y) || direction.y > 0)
            {
                offset = direction.x > 0 ?  Vector2Int.left : Vector2Int.right;
            }
            else
            {
                offset = Vector2Int.up;
            }
            Vector3Int offsetCellPos = lastSelectedCell + (Vector3Int)offset;
            TileBase offsetTile = tilemap.GetTile(offsetCellPos);

            if (offsetTile == null || !tileStore[offsetTile].pathable)
            {
                if (offset == Vector2Int.up)
                {
                    offset = direction.x > 0 ? Vector2Int.right : Vector2Int.left;
                }
                else
                    offset =  Vector2Int.up;
            }

            offsetCellPos = lastSelectedCell + (Vector3Int)offset;


            return (tilemap.GetCellCenterWorld(offsetCellPos) + worldPoint)/2;
        }

        public virtual void AreaDragged(Vector3 worldPoint)
        {
            lastDraggedPosition = worldPoint;
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