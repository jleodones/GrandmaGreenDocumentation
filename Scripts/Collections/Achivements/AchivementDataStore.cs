using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen
{
    public struct AchivementState
    {
        public string achivementName;
        public int progression;
    }

    [System.Serializable]
    public class GameAchivement
    {
        [field: SerializeField] public string name { get; protected set; } = string.Empty;
        [field: SerializeField] public bool completed { get; protected set; } = false;
        [field: SerializeField] public int progression { get; protected set; } = 0;
        [field: SerializeField] public int goalProgression { get; private set;}
        [SerializeField] AchivementRequirement requirement;
        [HideInInspector] public int index;

        public System.Action<GameAchivement> onCompletion;

        public void RegisterRequirement()
        {
            requirement.RegisterAchivement(this);
        }

        public void DeregisterRequirement()
        {
            requirement.DeregisterAchivement(this);
        }

        public void SetProgression(int value)
        {
            progression = value;
            if (progression >= goalProgression)
                completed = true;
        }

        public void AddProgression(int value)
        {
            progression += value;
            if (progression >= goalProgression)
                CompleteAchivement();
        }

        private void CompleteAchivement()
        {
            completed = true;
            DeregisterRequirement();
            onCompletion?.Invoke(this);
        }
    }

    [CreateAssetMenu(menuName = "GrandmaGreen/Achivements/Achivement Store")]
    public class AchivementDataStore : SaveSystem.ObjectSaver
    {
        public List<AchivementRequirement> requirements;
        public List<GameAchivement> achivements;

        Dictionary<string, GameAchivement> achivementLookup;

        public System.Action<GameAchivement> onAchivementCompleted;

        public void Initalize()
        {
            foreach (AchivementRequirement requirement in requirements)
            {
                requirement.SetupAchivementRequirement();
            }

            achivementLookup = new Dictionary<string, GameAchivement>(achivements.Count);
            foreach (GameAchivement achivement in achivements)
            {
                achivementLookup.TryAdd(achivement.name, achivement);
                achivement.index = -1;
            }

            LoadAchivementProgress();

            foreach (GameAchivement achivement in achivements)
            {
                if (achivement.completed)
                    continue;

                achivement.onCompletion += HandleAchivementComplete;
                achivement.RegisterRequirement();
            }
        }

        public void Release()
        {
            foreach (AchivementRequirement requirement in requirements)
            {
                requirement.ReleaseAchivementRequirement();
            }

            SaveAchivementProgress();

            foreach (GameAchivement achivement in achivements)
            {
                if (achivement.completed)
                    continue;

                achivement.onCompletion -= HandleAchivementComplete;
                achivement.DeregisterRequirement();
            }
        }

        public void LoadAchivementProgress()
        {
            if (GetComponentStore<AchivementState>() == null)
                return;

            AchivementState achivementState = default;
            for (int i = 0; i < achivements.Count; i++)
            {
                if (RequestData<AchivementState>(i, ref achivementState))
                {
                    achivementLookup[achivementState.achivementName].SetProgression(achivementState.progression);
                    achivementLookup[achivementState.achivementName].index = i;
                }
            }
        }

        public void SaveAchivementProgress()
        {
            if (GetComponentStore<AchivementState>() == null)
                CreateNewStore<AchivementState>();

            AchivementState achivementState;
            GameAchivement achivement;
            for (int i = 0; i < achivements.Count; i++)
            {
                achivement = achivements[i];

                achivementState.achivementName = achivement.name;
                achivementState.progression = achivement.progression;

                if (!UpdateValue<AchivementState>(achivement.index, achivementState))
                {
                    AddComponent<AchivementState>(-1, achivementState);
                }
            }
        }

        void HandleAchivementComplete(GameAchivement gameAchivement)
        {
            gameAchivement.onCompletion -= HandleAchivementComplete;
            onAchivementCompleted?.Invoke(gameAchivement);
        }
    }
}
