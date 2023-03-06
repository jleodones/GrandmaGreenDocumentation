using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;

namespace GrandmaGreen
{
    public enum StoryState
    {
        INACTIVE,
        ACTIVE,
        COMPLETED
    }

    [CreateAssetMenu(menuName = "GrandmaGreen/Storyline/Storyline")]
    public class Storyline : ScriptableObject
    {
        [SerializeField] public StoryRequirement[] requirements;

        [field: SerializeField] public uint progress { get; private set; }
        [field: SerializeField] public StoryState currentState { get; private set; }

        public event System.Action<Storyline> onStart;
        public event System.Action<Storyline> onProgress;
        public event System.Action<Storyline> onRegress;
        public event System.Action<Storyline> onCompletion;

        [ContextMenu("Progress Story")]
        public void ProgressStory()
        {
            RemoveCurrentRequirement();

            progress++;

            if (progress == requirements.Length)
            {
                CompleteStory();
                return;
            }

            SetupCurrentRequirement();

            onProgress?.Invoke(this);
        }

        [ContextMenu("Regress Story")]
        public void RegressStory()
        {
            if (progress == 0)
                return;

            if (currentState == StoryState.COMPLETED)
                currentState = StoryState.ACTIVE;
            else
            {
                requirements[progress].currentState = StoryState.INACTIVE;
                RemoveCurrentRequirement();
            }

            progress--;

            SetupCurrentRequirement();

            onRegress?.Invoke(this);
        }

        public void SetProgress(uint newProgress, bool retroactive = false)
        {
            progress = newProgress;
            currentState = StoryState.INACTIVE;

            for (int i = 0; i < requirements.Length; i++)
            {
                requirements[i].currentState = StoryState.INACTIVE;

                if (i >= progress)
                    continue;

                if (!retroactive)
                    requirements[i].currentState = StoryState.COMPLETED;
                else
                    requirements[i].CompleteRequirement();
            }

            if (progress == requirements.Length)
                currentState = StoryState.COMPLETED;
        }

        public void ClearStory()
        {
            if (currentState != StoryState.COMPLETED)
                RemoveCurrentRequirement();

            currentState = StoryState.INACTIVE;

            progress = 0;
            for (int i = 0; i < requirements.Length; i++)
            {
                requirements[i].currentState = StoryState.INACTIVE;
                requirements[i].flag.Reset();
            }
        }

        public void StartStory()
        {
            if (currentState != StoryState.INACTIVE)
                return;

            currentState = StoryState.ACTIVE;

            SetupCurrentRequirement();

            onStart?.Invoke(this);
        }

        public void CompleteStory()
        {
            currentState = StoryState.COMPLETED;
            onCompletion?.Invoke(this);
        }

        void RemoveCurrentRequirement()
        {
            requirements[progress].flag.onFlagRaised -= requirements[progress].CompleteRequirement;
            requirements[progress].onCompletion -= ProgressStory;
        }
        void SetupCurrentRequirement()
        {
            requirements[progress].flag.onFlagRaised += requirements[progress].CompleteRequirement;
            requirements[progress].onCompletion += ProgressStory;
            requirements[progress].ActivateRequirement();
        }
    }

    [System.Serializable]
    public class StoryRequirement
    {
        public GameEventFlag flag;
        public StoryState currentState;

        public event System.Action onActivation;
        public event System.Action onCompletion;

        public void ActivateRequirement(bool retroactive = false)
        {
            currentState = StoryState.ACTIVE;
            onActivation?.Invoke();

            if (retroactive)
            {
                if (flag.raised)
                    CompleteRequirement();
            }
        }

        public void CompleteRequirement()
        {
            if (currentState != StoryState.ACTIVE)
                return;

            currentState = StoryState.COMPLETED;
            onCompletion?.Invoke();
        }
    }
}
