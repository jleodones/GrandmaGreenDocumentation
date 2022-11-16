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
        public float actionRange = 1;

        [Header("Entity variables")]
        public GameEntity entity;
        public AreaController currentArea;
        public StateMachine<EntityState> stateMachine => entity.entityStateMachine;
        public bool active = false;

        public Coroutine behaviorRoutine;

        System.Action<EntityController> actionQ;

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

        public virtual void CancelDestination()
        {
            entity.splineFollow.ForceStop();
        }

        public virtual void SetDestination(Vector3 worldPos)
        {
            if ((entity.CurrentPos() - worldPos).sqrMagnitude <= actionRange * actionRange)
            {
                ExcecuteEntityActions();
                return;
            }

            float3[] path = entity.CheckPath(worldPos);

            if (path != null)
            {
                entity.onEntityPathEnd += ExcecuteEntityActions;
                entity.StartCoroutine(entity.FollowPath(path));
            }
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

        public virtual void PauseBehaviour()
        {
            entity.StopCoroutine(behaviorRoutine);
        }

        public virtual void ResumeBehaviour()
        {
            if (!permissions.HasFlag(currentBehavior.prerequisites))
                return;
            behaviorRoutine = entity.StartCoroutine(currentBehavior.PerformInstance(this));
        }

        public virtual void QueueEntityAction(System.Action<EntityController> action)
        {
            actionQ += action;
        }

        public virtual void ClearActionQueue()
        {
            actionQ = null;
        }
        public virtual void ExcecuteEntityActions()
        {
            entity.onEntityPathEnd -= ExcecuteEntityActions;

            actionQ?.Invoke(this);

            ClearActionQueue();
        }
    }
}