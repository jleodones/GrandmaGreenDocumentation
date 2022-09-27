using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine.Splines;
using Core.FSM;


namespace GrandmaGreen.Entities
{
    public enum EntityState
    {
        Idle,
        MovingTo,
        PerformingAction
    }

    /// <summary>
    /// Core Game Entity Behavior
    /// Serves as the customer for various GG services (pathfinding, gardening, dialogue)
    /// Has no decision making logic, instead it is handled by its corresponding Entity Controller
    /// </summary>
    public class GameEntity : MonoBehaviour, IPathAgent
    {
        [Header("Entity References")]
        public EntityController controller;
        public Animator animator;

        [field: Header("Entity Variables")]
        public StateMachine<EntityState> entityStateMachine;// { get; protected set; }
        public Vector3 velocity;
        [Header("Pathing")]
        public SplineFollow splineFollow;
        [Range(0, 1)]
        public float smoothFactor = 0.2f;

        public IPathfinderServicer pathfinderServicer => IPathAgent.Servicer;
        public bool isPathing => splineFollow.isFollowing;
        Transition idleToMoving;
        Transition movingToIdle;

        float3[] pathableNodes;

        Vector3 prevPosition;

        public event System.Action<Vector3> onEntityMove;
        public System.Action<Vector3> onEntityActionStart;
        public System.Action<Vector3> onEntityActionEnd;

        void Awake()
        {
            controller.RegisterEntity(this);
            InitalizeStateMachine();
        }

        void Start()
        {
            entityStateMachine.Enter(EntityState.Idle);
            prevPosition = transform.position;
        }
        void OnDestroy()
        {

        }

        protected virtual void InitalizeStateMachine()
        {
            StateMachine.CreateStateMachine<EntityState>(out entityStateMachine);

            idleToMoving = entityStateMachine.AddTransition(EntityState.Idle, EntityState.MovingTo);
            movingToIdle = entityStateMachine.AddTransition(EntityState.MovingTo, EntityState.Idle);

            entityStateMachine.GetState(EntityState.Idle).onLogicUpdate += IdleLogic;
            entityStateMachine.GetState(EntityState.MovingTo).onLogicUpdate += MovingLogic;
        }

        protected virtual void OnEnable()
        {
            controller.StartController();
        }

        protected virtual void OnDisable()
        {
            controller.PauseController();
        }

        public void Update()
        {
            if (animator.runtimeAnimatorController != null)
            {
                SetAnimatorParameters();
            }
            entityStateMachine.LogicUpdate();
        }

        public void FixedUpdate()
        {
            velocity = transform.position - prevPosition;

            prevPosition = transform.position;
            entityStateMachine.PhysicsUpdate();
            onEntityMove?.Invoke(prevPosition);
        }

        void MovingLogic()
        {
            if (velocity.magnitude == 0)
                movingToIdle.Trigger();
        }

        void IdleLogic()
        {
            if (velocity.magnitude != 0)
                idleToMoving.Trigger();
        }

        void SetAnimatorParameters()
        {
            if (velocity.magnitude != 0)
            {
                animator.SetInteger("DIRECTION", velocity.x < 0 ? -1 : 1);
                animator.SetInteger("MOVEMENT", (int)Mathf.Ceil(velocity.magnitude));
            }
            else
                animator.SetInteger("MOVEMENT", 0);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="range">Max  range to path to</param>
        /// <returns>Array of pathable destinations in the range</returns>
        public float3[] CalculatePathable(int range)
        {
            int2 startPos;

            startPos.x = (int)math.round((CurrentPos().x - pathfinderServicer.PrimaryService.gridData.worldOrigin.x) / pathfinderServicer.PrimaryService.gridData.cellSize.x);
            startPos.y = (int)math.round((CurrentPos().y - pathfinderServicer.PrimaryService.gridData.worldOrigin.y) / pathfinderServicer.PrimaryService.gridData.cellSize.y);

            pathableNodes = pathfinderServicer.PrimaryService.BFS(startPos, range);

            return pathableNodes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endPos">Target location</param>
        /// <returns>Float array of path to follow. Empty if no path is found</returns>
        public float3[] CheckPath(int2 endPos)
        {
            int2 startPos;

            startPos.x = (int)math.round((CurrentPos().x - pathfinderServicer.PrimaryService.gridData.worldOrigin.x) / pathfinderServicer.PrimaryService.gridData.cellSize.x);
            startPos.y = (int)math.round((CurrentPos().y - pathfinderServicer.PrimaryService.gridData.worldOrigin.y) / pathfinderServicer.PrimaryService.gridData.cellSize.y);
            float3[] path = pathfinderServicer.PrimaryService.PathFindAStar(startPos, endPos);

            return path;
        }

        //TODO: optimize
        public float3[] CheckPath(Vector3 worldPos)
        {
            int2 endPos;
            endPos.x = (int)math.round((worldPos.x - pathfinderServicer.PrimaryService.gridData.worldOrigin.x) / pathfinderServicer.PrimaryService.gridData.cellSize.x);
            endPos.y = (int)math.round((worldPos.y - pathfinderServicer.PrimaryService.gridData.worldOrigin.y) / pathfinderServicer.PrimaryService.gridData.cellSize.y);

            return CheckPath(endPos);
        }

        /// <summary>
        /// Follows the given float3 path
        /// </summary>
        /// <param name="path"></param>
        public virtual IEnumerator FollowPath(float3[] path)
        {
            Spline spline = default(Spline);

            path[0] = transform.position;

            spline = pathfinderServicer.PrimaryService.settings.pathSmoothing ?
                    new Spline(SplinePathBuilder.BuildKnotsSmooth(path, smoothFactor), false) :
                    new Spline(SplinePathBuilder.BuildKnots(path), false);

            spline.Warmup();
            splineFollow.Play(spline);


            onEntityActionStart?.Invoke(path[path.Length - 1]);

            splineFollow.onRetarget += onEntityActionStart;

            while (splineFollow.isFollowing)
            {
                yield return null;
            }

            onEntityActionEnd?.Invoke(CurrentPos());
            splineFollow.onRetarget -= onEntityActionStart;
        }

        public virtual void CancelPath()
        {
            splineFollow.ForceStop();
        }

        public virtual Vector3 CurrentPos()
        {
            return transform.position;
        }


    }
}