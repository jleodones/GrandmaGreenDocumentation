using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Pathfinding;

namespace GrandmaGreen
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Area/Area Servicer")]
    /// <summary>
    /// Main connector for the  area controllers
    /// </summary>
    public class AreaServices : ScriptableObject, IPathfinderServicer
    {
        [Header("Variables")]
        [SerializeField][ReadOnly] AreaController[] areaControllerSet;
        [SerializeField][ReadOnly] int activeAreaIndex;
        public AreaController ActiveArea => areaControllerSet[activeAreaIndex];
        public AreaExitState exitState;

        int m_lastRegisteredArea;

        [ContextMenu("Start")]
        /// <summary> 
        /// Starts the various gardening systems
        /// </summary>
        public virtual void StartServices()
        {
            IPathAgent.SetServicer(this);
            IPathfinderServicer.s_activeServices = new Pathfinder[4];

            activeAreaIndex = -1;
            areaControllerSet = new AreaController[5];

            exitState.exitSide = Direction.SOUTH;
        }

        /// <summary>
        /// Stops the various gardening systems
        /// </summary>
        public virtual void StopServices()
        {
            IPathAgent.ClearServicer();
            //IPathfinderServicer.s_activeServices = null;
        }

        /// <summary>
        /// Active an area controller
        /// </summary>
        /// <param name="areaController"></param>
        public void RegisterAreaController(AreaController areaController)
        {
            if (areaController.areaIndex >= areaControllerSet.Length)
                return;

            areaControllerSet[areaController.areaIndex] = areaController;
            m_lastRegisteredArea = areaController.areaIndex;
            RegisterService(areaController.pathfinder, areaController.areaIndex);
        }

        /// <summary>
        /// Deactivate an area controller
        /// </summary>
        /// <param name="areaController"></param>
        public void DesregisterAreaController(AreaController areaController)
        {
            if (areaController.areaIndex >= areaControllerSet.Length)
                return;

            areaControllerSet[areaController.areaIndex] = null;

            DeregisterService(areaController.pathfinder, areaController.areaIndex);

            if (m_lastRegisteredArea == areaController.areaIndex)
                m_lastRegisteredArea = -1;
        }

        /// <summary>
        /// Activates an area controller and sets it to the current active area
        /// Deactivates the previous active area
        /// </summary>
        /// <param name="areaIndex"></param>
        public void ActivateAreaController(int areaIndex)
        {
            if (areaIndex >= areaControllerSet.Length)
                return;

            if (areaIndex == -1 && m_lastRegisteredArea != -1)
                areaIndex = m_lastRegisteredArea;

            if (activeAreaIndex != -1)
                areaControllerSet[activeAreaIndex].Deactivate();

            activeAreaIndex = areaIndex;
            areaControllerSet[activeAreaIndex].Activate();
        }

        /// <summary>
        /// Deactivates the currently active area
        /// </summary>
        /// <param name="areaIndex"></param>
        public void DeactivateAreaController(int areaIndex)
        {
            if (areaIndex >= areaControllerSet.Length)
                return;

            areaControllerSet[areaIndex].Deactivate();

            if (areaIndex == 0)
                activeAreaIndex = -1;
            else
                activeAreaIndex = 0;
        }


        /// <summary>
        /// Placeholder method for registering a sample service
        /// </summary>
        /// <param name="pathfinder"></param>
        public virtual void RegisterService(Pathfinder pathfinder, int areaIndex)
        {

            IPathfinderServicer.s_activeServices[areaIndex] = pathfinder;
        }

        /// <summary>
        /// Placeholder method for registering a sample service
        /// </summary>
        /// <param name="pathfinder"></param>
        public virtual void DeregisterService(Pathfinder pathfinder, int areaIndex)
        {
            IPathfinderServicer.s_activeServices[areaIndex] = null;
        }
    }
}
