using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine.Splines;
using Core.FSM;


namespace GrandmaGreen
{
    public enum EntityState
    {
        Idle,
        MovingTo,
        PerformingAction
    }

    public class GameEntity : MonoBehaviour, IPathAgent
    {
        [Header("Entity References")]
        public EntityController controller;
        public Animator animator;

        [field: Header("Entity Variables")]
        public StateMachine<EntityState> entityStateMachine;// { get; protected set; }

        [Header("Pathing")]
        public SplineFollow splineFollow;
        [Range(0, 1)]
        public float smoothFactor = 0.2f;

        public IPathfinderServicer pathfinderServicer => IPathAgent.Servicer;
        public bool isPathing => throw new System.NotImplementedException();

        Transition idleToMoving;
        Transition movingToIdle;


        void Start()
        {
            InitalizeStateMachine();
            controller.RegisterEntity(this);

            entityStateMachine.Enter(EntityState.Idle);
        }
        void OnDestroy()
        {

        }

        protected virtual void InitalizeStateMachine()
        {
            StateMachine.CreateStateMachine<EntityState>(out entityStateMachine);


            idleToMoving = entityStateMachine.AddTransition(EntityState.Idle, EntityState.MovingTo);
            movingToIdle = entityStateMachine.AddTransition(EntityState.MovingTo, EntityState.Idle);


            splineFollow.onComplete += movingToIdle.Trigger;
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
            entityStateMachine.LogicUpdate();
        }

        public void FixedUpdate()
        {
            entityStateMachine.PhysicsUpdate();
        }

        public float3[] RequestPath(int2 startPos, int2 endPos)
        {
            float3[] path = pathfinderServicer.PrimaryService.PathFindAStar(startPos, endPos);

            return path;
        }

        //TODO: optimize
        public float3[] RequestPath(Vector3 worldPos)
        {
            int2 startPos;
            int2 endPos;

            startPos.x = (int)math.round((CurrentPos().x - pathfinderServicer.PrimaryService.gridData.worldOrigin.x) / pathfinderServicer.PrimaryService.gridData.cellSize.x);
            startPos.y = (int)math.round((CurrentPos().y - pathfinderServicer.PrimaryService.gridData.worldOrigin.y) / pathfinderServicer.PrimaryService.gridData.cellSize.y);

            endPos.x = (int)math.round((worldPos.x - pathfinderServicer.PrimaryService.gridData.worldOrigin.x) / pathfinderServicer.PrimaryService.gridData.cellSize.x);
            endPos.y = (int)math.round((worldPos.y - pathfinderServicer.PrimaryService.gridData.worldOrigin.y) / pathfinderServicer.PrimaryService.gridData.cellSize.y);

            return RequestPath(startPos, endPos);
        }

        public virtual void FollowPath(float3[] path)
        {
            Spline spline = default(Spline);

            path[0] = transform.position;

            spline = pathfinderServicer.PrimaryService.settings.pathSmoothing ?
                    new Spline(SplinePathBuilder.BuildKnotsSmooth(path, smoothFactor), false) :
                    new Spline(SplinePathBuilder.BuildKnots(path), false);

            spline.Warmup();
            splineFollow.Play(spline);

            idleToMoving.Trigger();
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