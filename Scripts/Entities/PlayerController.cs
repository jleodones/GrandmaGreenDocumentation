using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Input;

namespace GrandmaGreen.Entities
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Entities/Controllers/Player")]
    public class PlayerController : EntityController
    {
        public override void StartController()
        {
            TouchInteraction.OnInteraction += CheckMove;
            active = true;

            base.StartController();
        }

        public override void PauseController()
        {
            TouchInteraction.OnInteraction -= CheckMove;
            base.StartController();
        }

        void CheckMove(IGameInteractable interactable)
        {

        }
    }
}