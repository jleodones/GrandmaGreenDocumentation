using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Input;

namespace GrandmaGreen.Garden
{
    public class GardenDecorItem : MonoBehaviour
    {
        public Collider interactable;
        public BoxCollider boundsCollider;
        public SpriteRenderer sprite;
        public Collections.DecorationId decorID;
        [SerializeField] GardenCustomizer customizer;

        public System.Action<GardenDecorItem> onInteraction;

        public void SendInteractionAction() => onInteraction?.Invoke(this);

        public void EnableInteraction() => interactable.enabled = true;
        public void DisableInteraction() => interactable.enabled = false;

        
        void OnEnable()
        {
            customizer.onCustomizationStart += DisableInteraction;
            customizer.onCustomizationEnd += EnableInteraction;
        }

        void OnDisable()
        {
            customizer.onCustomizationStart -= DisableInteraction;
            customizer.onCustomizationEnd -= EnableInteraction;
        }
    }
}
