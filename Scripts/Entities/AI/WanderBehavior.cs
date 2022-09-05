using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GrandmaGreen.Entities
{

    [CreateAssetMenu(menuName = "GrandmaGreen/Entities/Wander Berhavior")]
    public class WanderBehavior : EntityBehavior
    {
        public float delay = 3;
        public int range = 30;
        public override IEnumerator PerformInstance(EntityController controller)
        {
            while (controller.currentBehavior == this)
            {
                yield return new WaitForSeconds(delay);

                controller.SetDestination(controller.FindRandomDestination(range));

                while (controller.stateMachine.activeState == EntityState.MovingTo)//TODO: remove dependency on entity FSM
                    yield return null;

            }
        }
    }
}