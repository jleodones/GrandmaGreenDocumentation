using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Input;

namespace GrandmaGreen
{
    [CreateAssetMenu()]
    public class PlayerController : EntityController
    {
        public override void StartController()
        {
            TouchInteraction.OnInteraction += CheckMove;
            active = true;
        }

        public override void PauseController()
        {
            TouchInteraction.OnInteraction -= CheckMove;
            active = false;
        }

        void CheckMove(IGameInteractable interactable)
        {

        }
    }
}