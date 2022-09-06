using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace GrandmaGreen.Garden
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Garden/Garden Area Servicer")]
    /// <summary>
    /// Main connector for the garden area controllers
    /// </summary>
    public class GardenAreaServicer : ScriptableObject, IPathfinderServicer
    {

        [ContextMenu("Start")]
        /// <summary>
        /// Starts the various gardening systems
        /// </summary>
        public void StartServices()
        {
            IPathAgent.SetServicer(this);
            IPathfinderServicer.s_activeServices = new List<Pathfinder>();
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
        public void RegisterAreaController(GardenAreaController areaController)
        {
            RegisterService(areaController.pathfinder);
        }

        /// <summary>
        /// Deactivate an area controller
        /// </summary>
        /// <param name="areaController"></param>
        public void DesregisterAreaController(GardenAreaController areaController)
        {
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
    }
}