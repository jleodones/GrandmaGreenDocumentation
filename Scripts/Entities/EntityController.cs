using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.Mathematics;
using Core.Utilities;
using Core.FSM;

namespace GrandmaGreen.Entities
{
    [System.Flags]
    /// <summary>
    /// Bit flag for all possible services this entity could use
    /// </summary>
    public enum EntityPermissions
    {
        None = 0,
        Pathfinder = 1,
        Gardener = 2,
        Speaker = 4,
        Interactable = 8
    }

    [CreateAssetMenu(menuName = "GrandmaGreen/Entities/Controllers/Default")]
    /// <summary>
    /// Brain of a GameEntity
    /// Entry point for all Entity logic
    /// </summary>
    public class EntityController : ScriptableObject
    {
        [Header("Entity Settings")]
        public EntityPermissions permissions;
        public EntityBehavior currentBehavior;

        [Header("Entity variables")]
        public GameEntity entity;
        public StateMachine<EntityState> stateMachine => entity.entityStateMachine;
        public bool active = false;

        public Coroutine behaviorRoutine;

        public virtual void RegisterEntity(GameEntity entity)
        {
            this.entity = entity;
        }


        public virtual void StartController()
        {
            active = true;

            if (currentBehavior != null)
                SetBehavior(currentBehavior);
        }

        public virtual void PauseController()
        {
            active = false;
            
        }

        public virtual void SetDestination(Vector3 worldPos)
        {
            float3[] path = entity.CheckPath(worldPos);

            if (path != null)
                entity.FollowPath(path);
        }

        public virtual void SetDestination(int2 endPos)
        {
            float3[] path = entity.CheckPath(endPos);

            if (path != null)
                entity.FollowPath(path);
        }

        public virtual float3 FindRandomDestination(int range)
        {
            float3[] pathable = entity.CalculatePathable(range);

            return pathable[UnityEngine.Random.Range(0, pathable.Length)];
        }

        public virtual void SetBehavior(EntityBehavior behavior)
        {
            if (!permissions.HasFlag(behavior.prerequisites))
                return;

            currentBehavior = behavior;

            behaviorRoutine = entity.StartCoroutine(currentBehavior.PerformInstance(this));
        }
    }
}