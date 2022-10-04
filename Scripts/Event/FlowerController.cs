// This file is only used for Event Manager Prototype

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GrandmaGreen {
    public class FlowerController : MonoBehaviour//, IGameInteractable
    {
        private void OnTriggerEnter(Collider other) {
            EventManager.instance.HandleEVENT_GOLEM_SPAWN(transform.parent.gameObject.GetInstanceID(),
                                                        transform.position);    
            Destroy(gameObject);
        }

        // public UnityEvent<Vector3> OnInteraction;
        // public void DoInteraction(Vector3 interactionPoint, PointerState interactionState)
        // {
        //     OnInteraction.Invoke(interactionPoint);
        // }    
}
}