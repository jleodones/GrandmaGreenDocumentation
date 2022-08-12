using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen
{
    [CreateAssetMenu()]
    public class AIController : EntityController
    {
        public List<EntityAction> ActionList;
        [SerializeField] ActionPlanner planner;

        public override void StartController()
        {
            planner = new ActionPlanner(ActionList);
            base.StartController();
        }
    }
}

