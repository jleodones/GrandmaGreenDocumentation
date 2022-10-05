using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using GrandmaGreen.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using Vector3 = UnityEngine.Vector3;

namespace GrandmaGreen
{
    /// <summary>
    /// Attach to dialogue-capable entities. This spawns their own dialogue bubble at the right time.
    /// </summary>
    public class DialoguePrototype : MonoBehaviour
    {
        private VisualElement m_dialogueRoot;
        public bool m_isSpeaking = false;

        void Start()
        {
            m_dialogueRoot = GetComponent<UIDocument>().rootVisualElement;
        }

        public void OnInteractDialogue(Vector3 interactionPoint)
        {
            if (!m_isSpeaking)
            {
                SpawnDialogue();
            }
            else
            {
                EndDialogue();
            }
        }

        private void SpawnDialogue()
        {
            // If entity is pathing, stop movement.
            EntityController entityController = GetComponentInParent<GameEntity>().controller;
            entityController.PauseBehaviour();

            // Turn on the dialogue bubble visuals.
            // This is so ugly...please fix this.
            SpriteRenderer entitySprite = transform.parent.gameObject.GetComponentInChildren<SpriteRenderer>();
            // worldPosition.x -= (entitySprite.sprite.bounds.size.x) / 2;
            // worldPosition.y -= (entitySprite.sprite.bounds.size.y);
            Vector3 worldPosition = entityController.entity.CurrentPos();
            
            SetPosition(worldPosition);
            m_dialogueRoot.Q("dialogueparent").style.display = DisplayStyle.Flex;
            m_isSpeaking = true;
        }

        private void EndDialogue()
        {
            // Free up movement again?
            EntityController entityController = GetComponentInParent<GameEntity>().controller;
            entityController.ResumeBehaviour();
            
            // Turn off the dialogue bubble visuals.
            m_dialogueRoot.Q("dialogueparent").style.display = DisplayStyle.None;
            m_isSpeaking = false;
        }
        
        void SetPosition(Vector3 worldSpace)
        {
            m_dialogueRoot.transform.position = RuntimePanelUtils.CameraTransformWorldToPanel(m_dialogueRoot.panel, worldSpace, Camera.main);
        }
    }
}
