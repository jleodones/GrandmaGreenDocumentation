using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Entities
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Entities/Follow Pointer Behavior")]
    public class FollowPointerBehavior : EntityBehavior
    {
        public override IEnumerator PerformInstance(EntityController controller)
        {
            throw new System.NotImplementedException();
        }

        public void SetPointerLocation(Vector3 position)
        {
            
        }

    }
}