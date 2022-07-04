using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.Mathematics;

namespace GrandmaGreen
{
    public class EntityController : ScriptableObject
    {
        [Header("Entity Settings")]
        public EntityPermissions permissions;

        [Header("Entity variables")]
        public GameEntity entity;
        public bool active = false;

        protected Pathfinder entityPathfinder;
        public virtual void RegisterEntity(GameEntity entity)
        {
            this.entity = entity;
        }

        public virtual void StartController()
        {
            active = true;
        }

        public virtual void PauseController()
        {
            active = false;
        }

        public virtual void MoveTo(Vector3 worldPos)
        {
            int2 startPos;
            int2 endPos;

            startPos.x = (int)math.round((entity.CurrentPos().x - entityPathfinder.gridData.worldOrigin.x) / entityPathfinder.gridData.cellSize.x);
            startPos.y = (int)math.round((entity.CurrentPos().y - entityPathfinder.gridData.worldOrigin.y) / entityPathfinder.gridData.cellSize.y);

            endPos.x = (int)math.round((worldPos.x - entityPathfinder.gridData.worldOrigin.x) / entityPathfinder.gridData.cellSize.x);
            endPos.y = (int)math.round((worldPos.y - entityPathfinder.gridData.worldOrigin.y) / entityPathfinder.gridData.cellSize.y);

            MoveTo(startPos, endPos);
        }

        public virtual void MoveTo(int2 startPos, int2 endPos)
        {
            float3[] path = entity.RequestPath(startPos, endPos);

            if (path.Length > 0)
                entity.FollowPath(path);
        }
    }
}