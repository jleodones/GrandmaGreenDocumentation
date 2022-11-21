using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GrandmaGreen.Garden;
using NaughtyAttributes;
using GrandmaGreen.Collections;
using GrandmaGreen.TimeLayer;
using Core.SceneManagement;

namespace GrandmaGreen
{

    /// <summary>
    /// 
    /// </summary>
    public class GrandmaGreenGameState : MonoBehaviour
    {
        protected static GrandmaGreenGameState s_Instance;

        [SerializeField] CollectionsSO collectionsData;
        [SerializeField] AreaServices areaServicer;
        [SerializeField] TimeLayerClock timeClock;
        [SerializeField] StorylineDataStore storylineData;
        [SerializeField] SaveSystem.SaveManager saveManager;
        [SerializeField] UI.UIDisplayTracker UIdisplayRules;
        [SerializeField] SCENES currentScene;
        [ReadOnly] int activeAreaIndex;
        [ReadOnly] bool isPaused;

        public static AreaController ActiveArea => s_Instance.areaServicer.ActiveArea;
        public static SCENES CurrentLocation => s_Instance.currentScene;

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

        void OnEnable()
        {
            SceneHandler.onSceneLoaded += SetCurrentScene;
        }

        void OnDisable()
        {
            SceneHandler.onSceneLoaded -= SetCurrentScene;
        }

        void OnDestroy()
        {
            areaServicer.StopServices();

            if (s_Instance != this)
                return;

            ReleaseState();

            s_Instance = null;
        }

        void InitalizeState()
        {
            UIdisplayRules.Initalize();
            collectionsData.LoadCollections();
            areaServicer.StartServices();
            storylineData.Initalize();
        }

        void ReleaseState()
        {
            UIdisplayRules.Release();
            collectionsData.UnloadCollections();
            timeClock.SaveCurrentDateTime();
            storylineData.Release();
            saveManager.TriggerSave();
        }

        void Start()
        {
            timeClock.SetClock();
            
        }

        void Update()
        {
            timeClock.TickClock(Time.deltaTime);
        }

        public void LoadGardenScreen(int gardenIndex)
        {
            areaServicer.ActivateAreaController(gardenIndex);
        }


        void SetCurrentScene(SCENES scene)
        {
            LoadGardenScreen(-1);
            currentScene = scene;
        }

        public void GameSceneTransition()
        {

        }
    }
}