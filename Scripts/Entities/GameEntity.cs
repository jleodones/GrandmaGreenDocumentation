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
        Moving,
        Busy
    }

    public class GameEntity : MonoBehaviour, IPathAgent
    {
        [Header("Entity References")]
        public EntityController controller;
        public Animator animator;

        [field: Header("Entity Variables")]
        public StateMachine<EntityState> entityStateMachine;// { get; protected set; }


        [Header("Pathing")]
        public SplineContainer splineContainer;
        public SplineFollow splineFollow;
        [Range(0, 1)]
        public float smoothFactor = 0.2f;

        public IPathfinderServicer pathfinderServicer => IPathAgent.Servicer;
        public bool isPathing => throw new System.NotImplementedException();

        void Start()
        {
            StateMachine.LoadStateMachine<EntityState>(out entityStateMachine);
            controller.RegisterEntity(this);

            entityStateMachine.Enter(EntityState.Idle);
        }
        void OnDestroy()
        {

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

        public float3[] RequestPath(Vector3 worldPos)
        {
            int2 startPos;
            int2 endPos;

            startPos.x = (int)math.round((CurrentPos().x - pathfinderServicer.PrimaryService.gridData.worldOrigin.x) / pathfinderServicer.PrimaryService.gridData.cellSize.x);
            startPos.y = (int)math.round((CurrentPos().y - pathfinderServicer.PrimaryService.gridData.worldOrigin.y) / pathfinderServicer.PrimaryService.gridData.cellSize.y);

            endPos.x = (int)math.round((worldPos.x - pathfinderServicer.PrimaryService.gridData.worldOrigin.x) /pathfinderServicer.PrimaryService.gridData.cellSize.x);
            endPos.y = (int)math.round((worldPos.y - pathfinderServicer.PrimaryService.gridData.worldOrigin.y) / pathfinderServicer.PrimaryService.gridData.cellSize.y);
            
            return RequestPath(startPos,endPos);
        }
        public float3[] RequestPath(int2 startPos, int2 endPos)
        {
            float3[] path = pathfinderServicer.PrimaryService.PathFindAStar(startPos, endPos);

            return path;
        }

        public virtual void FollowPath(float3[] path)
        {
            Spline spline = default(Spline);

            path[0] = transform.position;

            if (pathfinderServicer.PrimaryService.settings.pathSmoothing)
                spline = new Spline(SplinePathBuilder.BuildKnotsSmooth(path, smoothFactor), false);
            else
                spline = new Spline(SplinePathBuilder.BuildKnots(path), false);

            splineContainer.Spline = spline;
            spline.Warmup();

            splineFollow.Play();
        }

        public virtual Vector3 CurrentPos()
        {
            return transform.position;
        }
    }
}