using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen
{
    [System.Serializable]
    public class ActionPlanner
    {   
        public List<EntityAction> ActionList;

        public Queue<EntityAction> actions;


        public ActionPlanner(List<EntityAction> actionList)
        {
            ActionList = actionList;
        }
    }
}