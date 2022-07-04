using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace GrandmaGreen
{
    [CreateAssetMenu()]
    public class GardenAreaServicer : ScriptableObject, IPathfinderServicer
    {
        
        [ContextMenu("Start")]
        public void StartServices()
        {
            IPathAgent.SetServicer(this);
            IPathfinderServicer.s_activeServices = new List<Pathfinder>();
        }

        public void StopServices()
        {
            IPathAgent.ClearServicer();
            IPathfinderServicer.s_activeServices = null;
        }

        public void AddAreaController(GardenAreaController areaController)
        {
            RegisterService(areaController.pathfinder);
        }

        public void RemoveController(GardenAreaController areaController)
        {
            DeregisterService(areaController.pathfinder);
        }

        public void RegisterService(Pathfinder pathfinder)
        {
            if (!IPathfinderServicer.s_activeServices.Contains(pathfinder))
                IPathfinderServicer.s_activeServices.Add(pathfinder);
        }

        public void DeregisterService(Pathfinder pathfinder)
        {
            if (IPathfinderServicer.s_activeServices.Contains(pathfinder))
                IPathfinderServicer.s_activeServices.Remove(pathfinder);
        }
    }
}