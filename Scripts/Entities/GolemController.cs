using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine.Splines;
using NPBehave;
using SpookuleleAudio;

namespace GrandmaGreen.Entities
{
    /// <summary>
    /// Core Game Entity Behavior
    /// Serves as the customer for various GG services (pathfinding, gardening, dialogue)
    /// Has no decision making logic, instead it is handled by its corresponding Entity Controller
    /// </summary>
    public class GolemController : MonoBehaviour, IPathAgent
    {
        [Header("Entity References")]
        public EntityPermissions permissions;
        public Animator animator;
        public ASoundContainer sfx_Footsteps;
        public int range = 30;
        public float delay = 5f;

        [Header("Behaviour Tree")]
        private Blackboard blackboard;
        private Root behaviorTree;
        const string BK_PREV_POSITION = "prevPosition";
        const string BK_WANDER_POSITION = "wanderPosition";

        [field: Header("Entity Variables")]
        public Vector3 velocity;
        [Header("Pathing")]
        public SplineFollow splineFollow;
        [Range(0, 1)]
        public float smoothFactor = 0.2f;

        public IPathfinderServicer pathfinderServicer => IPathAgent.Servicer;
        public bool isPathing => splineFollow.isFollowing;

        #region private variables
        float3[] pathableNodes;

        Vector3 prevPosition;
        SoundPlayer footstepsPlayer;
        #endregion

        public event System.Action<Vector3> onEntityMove;
        public System.Action<Vector3> onEntityActionStart;
        public System.Action<Vector3> onEntityActionEnd;

        void Start()
        {
            prevPosition = transform.position;
            
            // create our behaviour tree and get it's blackboard
            behaviorTree = CreateBehaviourTree();
            blackboard = behaviorTree.Blackboard;
        
            // initialize blackboard
            behaviorTree.Blackboard.Set(BK_PREV_POSITION, transform.position);

            // attach the debugger component if executed in editor (helps to debug in the inspector) 
            #if UNITY_EDITOR
                Debugger debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
                debugger.BehaviorTree = behaviorTree;
            #endif

                // start the behaviour tree
                behaviorTree.Start();

        }

        public void Update()
        {
            if (animator && animator.runtimeAnimatorController != null)
            {
                SetAnimatorParameters();
            }
        }

        public void FixedUpdate()
        {
            velocity = transform.position - prevPosition;

            prevPosition = transform.position;
            onEntityMove?.Invoke(prevPosition);
        }

        private Root CreateBehaviourTree()
        {
        // we always need a root node
        return new Root(
            // park until playerDistance does change
                new Sequence(
                    new Cooldown(delay, new Action(() => {
                            Vector3 randPos = FindRandomDestination(range);
                            behaviorTree.Blackboard.Set(BK_WANDER_POSITION,randPos); 
                        })),
                    
                    new Action((bool _shouldCancel) =>
                        {
                            if (!_shouldCancel)
                            {
                                Vector3 pos = blackboard.Get<Vector3>(BK_WANDER_POSITION);
                                SetDestination(pos);
                                if ((pos-transform.position).magnitude < 0.1f) {
                                    return Action.Result.SUCCESS;
                                }
                                return Action.Result.PROGRESS;
                            } else {
                                return Action.Result.FAILED;
                            }
                        }) { Label = "Wander" }
                )
        );
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

        public virtual void SetDestination(Vector3 worldPos)
        {
            float3[] path = CheckPath(worldPos);

            if (path != null)
                StartCoroutine(FollowPath(path));
        }

         public virtual float3 FindRandomDestination(int range)
        {
            float3[] pathable = CalculatePathable(range);

            return pathable[UnityEngine.Random.Range(0, pathable.Length)];
        }

        #region Path finding service 
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

            if(!splineFollow.wasForceStopped)
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
        #endregion
    }
}