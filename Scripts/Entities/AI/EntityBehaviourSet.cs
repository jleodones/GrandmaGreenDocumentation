using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Entities
{
    public struct EntityBehaviorSetting
    {

    }

    [CreateAssetMenu(menuName = "GrandmaGreen/Entities/Entity Behavioural Set")]
    public class EntityBehaviorSet : EntityBehavior
    {
        public List<EntityBehavior> Behaviours;

        public override IEnumerator PerformInstance(EntityController controller)
        {
            foreach (EntityBehavior behavior in Behaviours)
            {
                if (!prerequisites.HasFlag(behavior.prerequisites))
                    continue;

                yield return behavior.PerformInstance(controller);
            }
        }
    }
}