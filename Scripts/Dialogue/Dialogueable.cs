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
    public enum DialogueMode
    {
        Tutorial,
        Idle,
        Story
    }
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

        public UnityAction onFinishDialogue;

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

        public DialogueMode dialogueMode = DialogueMode.Idle;
        void Awake()
        {
            // Search and store references to dialogue runner and dialogue line view.
            m_dialogueRunner = FindObjectOfType<DialogueRunner>();
            m_lineView = FindObjectOfType<CustomLineView>();
            
            // TODO: 

            onFinishDialogue += Finish;
        }

        /// <summary>
        /// Triggers dialogue, sending the yarn project to the dialogue runner.
        /// Tells it which node to start on, which is determined randomly for normal dialogue events.
        /// </summary>
        public void TriggerDialogue()
        {
            // Set yarn project and dialogue sounds.
            m_dialogueRunner.SetProject(yarnProject);
            m_lineView.dialogueSFX = dialogueSFX;
            
            // Add closing event.
            m_dialogueRunner.onDialogueComplete.AddListener(onFinishDialogue);
            
            // Calculate the starting node. For now, it's just idle dialogue.
            string nodeStart = "";
            switch (dialogueMode)
            {
                case DialogueMode.Tutorial:
                    nodeStart = "Tutorial";
                    break;
                case DialogueMode.Idle:
                    nodeStart = "Idle_";
                    int rand = (int)Random.Range(1, idleDialogueCount + 1);
                    nodeStart += rand.ToString();
                    break;
                case DialogueMode.Story:
                    break;
            }

            m_dialogueRunner.StartDialogue(nodeStart);
        }

        public void TriggerDialogueByNode(string s)
        {
            // Set yarn project and dialogue sounds.
            m_dialogueRunner.SetProject(yarnProject);
            m_lineView.dialogueSFX = dialogueSFX;
            
            // Add closing event.
            m_dialogueRunner.onDialogueComplete.AddListener(onFinishDialogue);
            
            m_dialogueRunner.StartDialogue(s);
        }

        private void Finish()
        {
            if (interactionEventScript != null)
            {
                interactionEventScript.OnInteraction?.Invoke(new InteractionEventData());
            }
            m_dialogueRunner.onDialogueComplete.RemoveListener(onFinishDialogue);
        }
    }
}
