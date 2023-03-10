using System;
using Core.Input;
using Sirenix.OdinInspector;
using GrandmaGreen.Entities;
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

        private CameraZoom m_currCameraZoom;
        private GameObject m_npcCameraTar;
        private GameObject m_gramCameraTar;
        private bool m_targetIsGrandma = false;
        private bool m_dialogueIsActive = false;

        // Global dialogue runner.
        private DialogueRunner m_dialogueRunner;


        // Global line view.
        private EntityLineView m_lineView;

        public DialogueMode dialogueMode = DialogueMode.Idle;
        void Awake()
        {
            // Search and store references to dialogue runner and dialogue line view.
            m_dialogueRunner = FindObjectOfType<DialogueRunner>();
            m_lineView = FindObjectOfType<EntityLineView>();
            
            onFinishDialogue += Finish;
        }

        private void Update()
        {
            if (m_dialogueIsActive)
            {
                string character = m_lineView.GetCharacterName();
                if (character == "Grandma")
                {
                    if (!m_targetIsGrandma)
                    {
                        PanCamera(true);
                    }
                }
                else
                {
                    if (m_targetIsGrandma)
                    {
                        PanCamera(false);
                    }
                }
            }
        }

        /// <summary>
        /// Called when starting dialogue, pauses the entity and zooms the camera in
        /// </summary>
        public void PauseAndZoomInEntity(float zoomAmount, CameraZoom currCameraZoom, GameObject npcCameraTar, GameObject gramCameraTar)
        {
            m_npcCameraTar = npcCameraTar;
            m_gramCameraTar = gramCameraTar;
            if (currCameraZoom)
            {
                m_currCameraZoom = currCameraZoom;
                m_currCameraZoom.ZoomCameraRequestNPC(3.85f, 0.5f, m_npcCameraTar);
            }
        }

        /// <summary>
        /// Called when finishing dialogue, resumes the entity and zooms the camera back out to normal
        /// </summary>
        /// <param name="entity"></param>
        public void ResumeAndZoomOutEntity()
        {

            m_currCameraZoom.ZoomCameraRequestNPC(5.0f, 0.5f, m_gramCameraTar);
        }

        /// <summary>
        /// NOTE: Only call this function if grandma is NOT the first to speak. Assumes that variables are already
        /// initialized from when the NPC first speaks.
        /// Pans camera to either grandma or golem depending on who is speaking
        /// </summary>
        public void PanCamera(bool grammaIsTalking)
        {
            if (grammaIsTalking)
            {
                m_targetIsGrandma = true;
                m_currCameraZoom.SetCameraFollow(m_gramCameraTar.transform);
            }
            else
            {
                m_targetIsGrandma = false;
                m_currCameraZoom.SetCameraFollow(m_npcCameraTar.transform);
            }
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
            m_dialogueIsActive = true;
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
            m_dialogueIsActive = false;
            if (m_currCameraZoom)
            {
                ResumeAndZoomOutEntity();
            }

            if (interactionEventScript != null)
            {
                interactionEventScript.OnInteraction?.Invoke(new InteractionEventData());
            }
            m_dialogueRunner.onDialogueComplete.RemoveListener(onFinishDialogue);
        }
    }
}
