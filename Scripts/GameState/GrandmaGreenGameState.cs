using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GrandmaGreen.Garden;
using NaughtyAttributes;
using GrandmaGreen.Collections;
using GrandmaGreen.TimeLayer;

namespace GrandmaGreen
{
    /// <summary>
    /// 
    /// </summary>
    public class GrandmaGreenGameState : MonoBehaviour
    {
        protected static GrandmaGreenGameState s_Instance;

        [SerializeField] CollectionsSO collectionsData;
        [SerializeField] GardenAreaServicer gardenServicer;
        [SerializeField] TimeLayerClock timeClock;
        [SerializeField] SaveSystem.SaveManager saveManager;
        [ReadOnly] int activeAreaIndex;

        public GardenAreaController ActiveArea => gardenServicer.ActiveArea;

        void Awake()
        {
            if (s_Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            s_Instance = this;

            saveManager.enabled = true;
            InitalizeState();
        }

        void OnDestroy()
        {
            gardenServicer.StopServices();

            if (s_Instance != this)
                return;

            timeClock.SaveCurrentDateTime();
            s_Instance = null;

            saveManager.TriggerSave();
        }

        void InitalizeState()
        {
            gardenServicer.StartServices();
        }

        void Start()
        {
            LoadGardenScreen(0);
            timeClock.SetClock();
        }

        void Update() 
        {
            timeClock.TickClock(Time.deltaTime);
        }
        
        public void LoadGardenScreen(int gardenIndex)
        {
            gardenServicer.ActivateAreaController(gardenIndex);
        }


    }
}