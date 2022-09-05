using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;

namespace GrandmaGreen.Entities
{

    //[CreateAssetMenu(menuName = "Entities/Entity Goal")]
    public abstract class EntityBehavior : ScriptableObject
    {
        [field: SerializeField] public EntityPermissions prerequisites{get; protected set;}

        public abstract IEnumerator PerformInstance(EntityController controller);
    }
}