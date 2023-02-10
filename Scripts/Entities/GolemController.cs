using System.Collections;
using System.Collections.Generic;
using Core.Input;
using GrandmaGreen.UI.Golems;
using GrandmaGreen.Collections;
using UnityEngine;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine.Splines;
using NPBehave;
using SpookuleleAudio;
using Sirenix.OdinInspector;

namespace GrandmaGreen.Entities
{
    /// <summary>
    /// Golem AI Controller driven by behaviour tree
    /// </summary>
    public class GolemController : MonoBehaviour, IPathAgent
    {
        [Header("Entity References")]
        public EntityPermissions permissions;
        public GameObject BabyRig;
        public GameObject MatureRig;
        public Animator animator;
        public ASoundContainer sfx_Footsteps;

        [Header("Behaviour Tree")]
        private Blackboard blackboard;
        private Root behaviorTree;
        const string BK_PREV_POSITION = "prevPosition";
        const string BK_WANDER_POSITION = "wanderPosition";

        [field: Header("Entity Variables")]
        public GolemManager golemManager;
        public CharacterId id;
        public bool isMature = false;
        public Vector3 velocity;
        public int range = 300;
        public float delay = 5f;
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

        private bool m_isInteracting = false;
        private bool m_isDragging = false;
        private bool m_isDoingTask = false;
        private float m_tapTime = float.MaxValue;
        private Vector3 m_originalPos;
        #endregion

        #region events
        // public event System.Action<bool> onEntityInteract;
        public event System.Action<Vector3> onEntityMove;
        public System.Action<Vector3> onEntityActionStart;
        public System.Action<Vector3> onEntityActionEnd;
        #endregion

        void Start()
        {
            prevPosition = transform.position;

            //Subscribe
            EventManager.instance.EVENT_GOLEM_EVOLVE += OnGolemEvolve;
            EventManager.instance.EVENT_GOLEM_RELEASE_SELECTED += OnGolemSelectRelease;
            EventManager.instance.EVENT_GOLEM_DO_TASK += UpdateTaskState;
            EventManager.instance.EVENT_GOLEM_ENABLE += EnableGolem;
            EventManager.instance.EVENT_GOLEM_DISABLE += DisableGolem;

            // create our behaviour tree and get it's blackboard
            behaviorTree = CreateBehaviourTree();
            blackboard = behaviorTree.Blackboard;
        
            // initialize blackboard
            InitializeBT();

            // attach the debugger component if executed in editor (helps to debug in the inspector) 
            #if UNITY_EDITOR
                Debugger debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
                debugger.BehaviorTree = behaviorTree;
            #endif

            // start the behaviour tree
            behaviorTree.Start();

        }

        private void OnDestroy() {
            EventManager.instance.EVENT_GOLEM_EVOLVE -= OnGolemEvolve;
            EventManager.instance.EVENT_GOLEM_RELEASE_SELECTED -= OnGolemSelectRelease;
            EventManager.instance.EVENT_GOLEM_DO_TASK -= UpdateTaskState;
            EventManager.instance.EVENT_GOLEM_ENABLE -= EnableGolem;
            EventManager.instance.EVENT_GOLEM_DISABLE -= DisableGolem;
            StopBehaviorTree();
        }

        public void Update()
        {
            if (animator && animator.runtimeAnimatorController != null)
            {
                SetAnimatorParameters();
            }

            velocity = transform.position - prevPosition;

            prevPosition = transform.position;
            onEntityMove?.Invoke(prevPosition);
        }

        public void FixedUpdate()
        {
            if (m_isInteracting && !m_isDragging)
            {
                // facing to grandma
                Vector3 playerPos = golemManager.GetPlayerPos();
                if (playerPos.x < transform.position.x) {
                    this.gameObject.transform.localScale = new Vector3(1, 1, 1);
                } else {
                    this.gameObject.transform.localScale = new Vector3(-1, 1, 1);
                }

                if ((playerPos - transform.position).sqrMagnitude <= 5.0f) {
                    GetComponentInChildren<GolemMenu>().ToggleMenu(true);
                } else {
                    GetComponentInChildren<GolemMenu>().ToggleMenu(false);
                }
            }

            if ((Time.time - m_tapTime > 0.5f))
            {
                m_tapTime = float.MaxValue;
                EnterDraggingState(true);
                EventManager.instance.HandleEVENT_GOLEM_DRAG(this.gameObject);
            }
        }

        private void InitializeBT() {
            behaviorTree.Blackboard.Set(BK_PREV_POSITION, transform.position);
            behaviorTree.Blackboard.Set("isInteract", m_isInteracting);
            behaviorTree.Blackboard.Set("isDoingTask", m_isDoingTask);
        }

        private Root CreateBehaviourTree()
        {
            // we always need a root node
            return new Root(
                    new Selector(
                        //1. onInteract
                        new BlackboardCondition("isInteract", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                            new Sequence(
                                new Action(() => {
                                    CancelPath();
                                }) { Label = "Change movement." },

                                new WaitUntilStopped()
                            )
                        ),

                        //2. Watering behavior
                        new BlackboardCondition("isDoingTask", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                            new Sequence(

                                // Move to new pos until arrival, and then water
                                new Action((bool inBound) =>
                                {
                                    if (!inBound)
                                    {
                                        Vector3 pos = golemManager.golemStateTable[(int)id - 5000].assignedCell;
                                        SetDestination(pos);
                                        if ((pos - transform.position).magnitude < 1f)
                                        {
                                            return Action.Result.SUCCESS;
                                        }
                                        return Action.Result.PROGRESS;
                                    }
                                    else
                                    {
                                        return Action.Result.FAILED;
                                    }
                                }) { Label = "Travel To Destination" },

                                new Action(() =>
                                {
                                    UpdateTaskState(-10);
                                }) { Label = "Water Cell"},

                                new WaitUntilStopped()
                            )
                        ),

                        //3. wandering behaviour
                        new Sequence(
                            // set wander pos
                            new Cooldown(delay, new Action(() => {
                                Vector3 randPos = FindRandomDestination(range);
                                behaviorTree.Blackboard.Set(BK_WANDER_POSITION, randPos);
                            })),

                            // move to new pos until arrival
                            new Action((bool _shouldCancel) =>
                            {
                                if (!_shouldCancel)
                                {
                                    Vector3 pos = blackboard.Get<Vector3>(BK_WANDER_POSITION);
                                    SetDestination(pos);
                                    if ((pos - transform.position).magnitude < 0.5f) {
                                        return Action.Result.SUCCESS;
                                    }
                                    return Action.Result.PROGRESS;
                                } else {
                                    return Action.Result.FAILED;
                                }
                            }) { Label = "Wander" }
                        )
                    )
                );
        } 


        public void StopBehaviorTree()
        {
            if ( behaviorTree != null && behaviorTree.CurrentState == Node.State.ACTIVE )
            {
                behaviorTree.Stop();
            }
        }

        void SetAnimatorParameters()
        {
            if (velocity.magnitude != 0 && m_isDragging == false)
            {

                 //early out if barely moving sideways, prevents flickering
                if(math.abs(velocity.x) < 0.001)
                {
                    animator.SetInteger("MOVEMENT", (int)Mathf.Ceil(velocity.magnitude));
                    return;
                }
                
                if (velocity.x < 0) {
                    this.gameObject.transform.localScale = new Vector3(1, 1, 1);
                } else {
                    this.gameObject.transform.localScale = new Vector3(-1, 1, 1);
                }
                animator.SetInteger("MOVEMENT", (int)Mathf.Ceil(velocity.magnitude));
            }
            else
                animator.SetInteger("MOVEMENT", 0);

        }
        
        public void UpdateInteractState() 
        {
            //Debug.Log("Interacting.");
            m_isInteracting = !m_isInteracting;
            behaviorTree.Blackboard.Set("isInteract", m_isInteracting);     
            if (m_isInteracting) {
                EventManager.instance.HandleEVENT_GOLEM_GRANDMA_MOVE_TO(transform.position);
            } else {
                GetComponentInChildren<GolemMenu>().ToggleMenu(false);
            }
        }

        public void EnterDraggingState(bool state)
        {
            m_isDragging = state;
            behaviorTree.Blackboard.Set("isInteract", m_isDragging);
            m_originalPos = transform.position;
            GetComponentInChildren<GolemMenu>().ToggleMenu(false);
        }

        public void ExitDragAttempt(bool attempt)
        {
            m_isDragging = false;
            m_isInteracting = false;
            behaviorTree.Blackboard.Set("isInteract", false);
            if (!attempt)
            {
                transform.position = m_originalPos;
            }
        }

        public void UpdateTaskState(int happinessValue)
        {
            if(golemManager.golemStateTable[offsetId(id)].assignedWatering)
            {       
                m_isDoingTask = !m_isDoingTask;
                behaviorTree.Blackboard.Set("isDoingTask", m_isDoingTask);
                Debug.Log("Task state assigned:" + m_isDoingTask);
                if(!m_isDoingTask){
                    EventManager.instance.HandleEVENT_WATER_PLANT(golemManager.golemStateTable[offsetId(id)].assignedCell);
                    EventManager.instance.HandleEVENT_GOLEM_HAPPINESS_UPDATE((ushort)id, happinessValue);
                }
            } else
            {
                Debug.Log("Some other task assigned");
            }
        }

        public void RegisterManager(GolemManager mgmr)
        {
            this.golemManager = mgmr;
        }

        #region Interaction
        public void OnGolemSelectRelease()
        {
            m_isInteracting = false;
            behaviorTree.Blackboard.Set("isInteract", m_isInteracting);
            GetComponentInChildren<GolemMenu>().ToggleMenu(false);
        }

        public void OnGolemTapped()
        {
            m_tapTime = Time.time;
        }

        public void OnGolemTapEnd()
        {
            float tappingTime = Time.time - m_tapTime;
            Debug.Log("End Interaction: " + (tappingTime));

            if (tappingTime > 0 && tappingTime < 0.5f){
                UpdateInteractState();
                m_tapTime = float.MaxValue;
            } 
        }
        #endregion

        #region Golem Evolution
        //Golem event handler
        public void OnGolemEvolve(ushort golemID) {
            if (id == (CharacterId)golemID) {
                Debug.Log(id.ToString() + " EVOLVE!");
                UpdateMatureState(true);
            }
        }

        //Golem sprite change
        public void UpdateMatureState(bool isGrowUp) {
            if (this.isMature == true) return;

            if (isGrowUp) {
                this.isMature = isGrowUp;
                BabyRig.SetActive(false);
                MatureRig.SetActive(true);
                animator = MatureRig.GetComponent<Animator>();
            }
        }

        //For debugging
        [ContextMenu("Evolve")]
        public void EvolveGolem()
        {
            UpdateMatureState(true);
        }
        #endregion

        #region  apis
        public ushort GetGolemID() {
            return (ushort)id;
        }

        public Bounds GetGolemBox() {
            if (isMature) {
                return MatureRig.GetComponentInChildren<Collider>().bounds;
            } else {
                return BabyRig.GetComponentInChildren<Collider>().bounds;
            }
        }
        #endregion
        
        //traverse
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

        public void EnableGolem()
        {
            gameObject.SetActive(true);
            behaviorTree.Start();
        }

        public void DisableGolem()
        {
            animator.SetInteger("MOVEMENT", 0);
            animator.Update(0f);
            behaviorTree.Stop();
            gameObject.SetActive(false);
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

        #region Utility
        private int offsetId(CharacterId id) {return (int)id - 5000;}

        public GameObject getActiveRig()
        {
            if (isMature)
                return MatureRig;
            else 
                return BabyRig;
        }
        #endregion
    }
}