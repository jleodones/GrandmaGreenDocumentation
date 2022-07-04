using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine.Splines;

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

    public abstract class GameEntity : MonoBehaviour, IPathAgent
    {
        [Header("Entity References")]
        public EntityController controller;
        public Animator animator;

        public IPathfinderServicer pathfinderServicer => IPathAgent.Servicer;
        public bool isPathing => throw new System.NotImplementedException();

        [Header("Pathing")]
        public SplineContainer splineContainer;
        public SplineFollow splineFollow;


        [Range(0, 1)]
        public float smoothFactor = 0.2f;

        protected virtual void Start()
        {
            controller.RegisterEntity(this);

        }
        protected virtual void OnDestroy()
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
        
        public float3[] RequestPath(int2 startPos, int2 endPos)
        {
            float3[] path = pathfinderServicer.PrimaryService.PathFindAStar(startPos,endPos);

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