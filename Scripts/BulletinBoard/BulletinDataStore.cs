using GrandmaGreen.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen
{    
    public struct BulletinState
    {
        public string bulletinName;
        public string bulletinType;

        public int rewardMoney;
        public int progression;
        public bool win;
    }

    [System.Serializable]
    public class GameBulletin
    {
        [field: SerializeField] public string Type { get; protected set; } = string.Empty;
        [field: SerializeField] public string Name { get; protected set; } = string.Empty;
        [field: SerializeField] public bool Completed { get; protected set; } = false;
        [field: SerializeField] public int Progression { get; protected set; } = 0;
        [field: SerializeField] public int GoalProgression { get; private set; }
        [field: SerializeField] public int RewardMoney { get; protected set; } = 0;
        [field: SerializeField] public bool WinState { get; protected set; } = false;

        [SerializeField] BulletinRequirement requirement;
        [HideInInspector] public int index;

        public System.Action<GameBulletin> onBulletinCompletion;

        public void RegisterBulletinRequirement()
        {
            requirement.RegisterBulletin(this);
        }

        public void DeregisterBulletinRequirement()
        {
            requirement.DeregisterBulletin(this);
        }

        public void SetProgression(int value)
        {
            Progression = value;
            if (Progression >= GoalProgression)
                Completed = true;
        }

        public void AddProgression(int value)
        {
            Progression += value;
            if (Progression >= GoalProgression)
                CompleteBulletin();
        }

        private void CompleteBulletin()
        {
            Completed = true;
            if (Progression == 1) WinState = true;
            else WinState = false;
            DeregisterBulletinRequirement();
            onBulletinCompletion?.Invoke(this);
        }
    }

    [CreateAssetMenu(menuName = "GrandmaGreen/Bulletin/BulletinDataStore")]
    public class BulletinDataStore : SaveSystem.ObjectSaver
    {
        public List<BulletinRequirement> requirements;
        public List<GameBulletin> bulletinBoardOptions;

        Dictionary<string, GameBulletin> bulletinLookup;

        public System.Action<GameBulletin> onBulletinCompleted;

        public void Initialize()
        {
            foreach (BulletinRequirement requirement in requirements)
            {
                requirement.SetupBulletinRequirement();
            }

            bulletinLookup = new Dictionary<string, GameBulletin>(bulletinBoardOptions.Count);
            foreach (GameBulletin bulletin in bulletinBoardOptions)
            {
                bulletinLookup.TryAdd(bulletin.Name, bulletin);
                bulletin.index = -1;
            }

            LoadBulletinProgress();

            foreach (GameBulletin bulletin in bulletinBoardOptions)
            {
                if (bulletin.Completed)
                    continue;

                bulletin.onBulletinCompletion += HandleBulletinComplete;
                bulletin.RegisterBulletinRequirement();
            }
        }

        public void Release()
        {
            foreach (BulletinRequirement requirement in requirements)
            {
                requirement.ReleaseBulletinRequirement();
            }

            SaveBulletinProgress();

            foreach (GameBulletin bulletin in bulletinBoardOptions)
            {
                if (bulletin.Completed)
                    continue;

                bulletin.onBulletinCompletion -= HandleBulletinComplete;
                bulletin.DeregisterBulletinRequirement();
            }
        }

        public void LoadBulletinProgress()
        {
            if (GetComponentStore<BulletinState>() == null)
                return;

            BulletinState bulletinState = default;
            for (int i = 0; i < bulletinBoardOptions.Count; i++)
            {
                if (RequestData<BulletinState>(i, ref bulletinState))
                {
                    bulletinLookup[bulletinState.bulletinName].SetProgression(bulletinState.progression);
                    bulletinLookup[bulletinState.bulletinName].index = i;
                }
            }
        }

        public void SaveBulletinProgress()
        {
            if (GetComponentStore<BulletinState>() == null)
                CreateNewStore<BulletinState>();

            BulletinState bulletinState;
            GameBulletin bulletin;
            for (int i = 0; i < bulletinBoardOptions.Count; i++)
            {
                bulletin = bulletinBoardOptions[i];

                bulletinState.bulletinName = bulletin.Name;
                bulletinState.progression = bulletin.Progression;

                /*if (!UpdateValue<BulletinState>(bulletin.index, bulletinState))
                {
                    AddComponent<BulletinState>(-1, bulletinState);
                }*/
            }
        }

        void HandleBulletinComplete(GameBulletin GameBulletin)
        {
            GameBulletin.onBulletinCompletion -= HandleBulletinComplete;
            onBulletinCompleted?.Invoke(GameBulletin);
        }
    }
}
