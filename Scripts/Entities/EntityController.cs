using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.Mathematics;

namespace GrandmaGreen
{
    [System.Flags]
    public enum EntityPermissions
    {
        None = 0,
        Pathfinder = 1,
        Gardener = 2,
        Speaker = 4,
        Interactable = 8
    }

    public class EntityController : ScriptableObject
    {
        [Header("Entity Settings")]
        public EntityPermissions permissions;

        [Header("Entity variables")]
        public GameEntity entity;
        public bool active = false;


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
            float3[] path = entity.RequestPath(worldPos);

            if (path.Length > 0)
                entity.FollowPath(path);
        }

        public virtual void MoveTo(int2 startPos, int2 endPos)
        {
            float3[] path = entity.RequestPath(startPos, endPos);

            if (path.Length > 0)
                entity.FollowPath(path);
        }
    }
}