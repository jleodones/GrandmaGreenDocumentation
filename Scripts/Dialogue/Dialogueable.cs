using System;
using Core.Input;
using SpookuleleAudio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Yarn.Unity;
using Random = UnityEngine.Random;

namespace GrandmaGreen.Dialogue
{
    public class Dialogueable : MonoBehaviour
    {
        /// <summary>
        /// This golem's yarn project, containing its dialogue.
        /// </summary>
        public YarnProject yarnProject;

        /// <summary>
        /// This golem's sound SFX, used for speech.
        /// </summary>
        public ListContainer dialogueSFX;

        public UnityAction<string> onFinishDialogue;

        /// <summary>
        /// Called to end dialogue and reset golem behavior.
        /// </summary>
        public InteractionEvent interactionEventScript;

        /// <summary>
        /// Number of idle dialogue nodes that this golem has.
        /// </summary>
        public int idleDialogueCount = 1;
        
        // Global dialogue runner.
        private DialogueRunner m_dialogueRunner;

        // Global line view.
        private CustomLineView m_lineView;
        void Awake()
        {
            // Search and store references to dialogue runner and dialogue line view.
            m_dialogueRunner = FindObjectOfType<DialogueRunner>();
            m_lineView = FindObjectOfType<CustomLineView>();

            onFinishDialogue += Finish;
        }

        /// <summary>
        /// Triggers dialogue, sending the yarn project to the dialogue runner.
        /// Tells it which node to start on, which is determined randomly for normal dialogue events.
        /// </summary>
        /// TODO: Register golem with the line view for camera switching when speaking.
        public void TriggerDialogue()
        {
            // Set yarn project and dialogue sounds.
            m_dialogueRunner.SetProject(yarnProject);
            m_lineView.dialogueSFX = dialogueSFX;
            
            // Add closing event.
            m_dialogueRunner.onNodeComplete.AddListener(onFinishDialogue);
            
            // Calculate the starting node. For now, it's just idle dialogue.
            // TODO: Check whether this should be a random dialogue or trigger an event.
            string nodeStart = "Idle_";
            int rand = (int)Random.Range(1, idleDialogueCount + 1);

            m_dialogueRunner.StartDialogue(nodeStart + rand.ToString());
        }

        private void Finish(string s)
        {
            interactionEventScript.OnInteraction?.Invoke(new InteractionEventData());
            m_dialogueRunner.onNodeComplete.RemoveListener(onFinishDialogue);
        }
    }
}
