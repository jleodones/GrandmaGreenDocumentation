using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen
{
    [System.Serializable]
    public abstract class EntityAction
    {
        public EntityStateData preconditions;
        public EntityStateData effects;
        public int cost;

        public abstract void PerformAction(EntityController entity);
    }
}