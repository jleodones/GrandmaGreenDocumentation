using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using NaughtyAttributes;

namespace GrandmaGreen.Garden
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Garden/Garden Area Servicer")]
    /// <summary>
    /// Main connector for the garden area controllers
    /// </summary>
    public class GardenAreaServicer : ScriptableObject, IPathfinderServicer
    {
        [SerializeField][ReadOnly] GardenAreaController[] areaControllerSet;
        [SerializeField][ReadOnly] int activeAreaIndex;
        [SerializeField] PlantStateManager globalPlantState;
        public GardenAreaController ActiveArea => areaControllerSet[activeAreaIndex];

        [ContextMenu("Start")]
        /// <summary>
        /// Starts the various gardening systems
        /// </summary>
        public void StartServices()
        {
            IPathAgent.SetServicer(this);
            IPathfinderServicer.s_activeServices = new List<Pathfinder>();

            activeAreaIndex = -1;
            areaControllerSet = new GardenAreaController[5];
            globalPlantState.Initialize();
        }

        /// <summary>
        /// Stops the various gardening systems
        /// </summary>
        public void StopServices()
        {
            IPathAgent.ClearServicer();
            IPathfinderServicer.s_activeServices = null;
        }

        /// <summary>
        /// Active an area controller
        /// </summary>
        /// <param name="areaController"></param>
        public void RegisterAreaController(GardenAreaController areaController, int areaIndex)
        {
            if (areaIndex >= areaControllerSet.Length)
                return;

            areaControllerSet[areaIndex] = areaController;

            RegisterService(areaController.pathfinder);
        }

        /// <summary>
        /// Deactivate an area controller
        /// </summary>
        /// <param name="areaController"></param>
        public void DesregisterAreaController(GardenAreaController areaController, int areaIndex)
        {
            if (areaIndex >= areaControllerSet.Length)
                return;

            areaControllerSet[areaIndex] = null;

            DeregisterService(areaController.pathfinder);
        }

        /// <summary>
        /// Placeholder method for registering a sample service
        /// </summary>
        /// <param name="pathfinder"></param>
        public void RegisterService(Pathfinder pathfinder)
        {
            if (!IPathfinderServicer.s_activeServices.Contains(pathfinder))
                IPathfinderServicer.s_activeServices.Add(pathfinder);
        }

        /// <summary>
        /// Placeholder method for registering a sample service
        /// </summary>
        /// <param name="pathfinder"></param>
        public void DeregisterService(Pathfinder pathfinder)
        {
            if (IPathfinderServicer.s_activeServices.Contains(pathfinder))
                IPathfinderServicer.s_activeServices.Remove(pathfinder);
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
            if (areaIndex >=areaControllerSet.Length)
                return;

            areaControllerSet[areaIndex].Deactivate();
            activeAreaIndex = -1;
        }
    }
}