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

        [SerializeField] GameSettingsData settingsData;
        [SerializeField] CollectionsSO collectionsData;
        [SerializeField] AreaServices areaServicer;
        [SerializeField] TimeLayerClock timeClock;
        [SerializeField] StorylineDataStore storylineData;
        [SerializeField] SaveSystem.SaveManager saveManager;
        [SerializeField] Entities.GolemManager golemManager;
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
        void Start()
        {


        }

        void Update()
        {
            timeClock.TickClock(Time.deltaTime);
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

        void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus)
            {
                Debug.Log("App Gained focus!");
                timeClock.SetClock();
            }
            else
            {
                Debug.Log("App Lost focus!");
                timeClock.SaveCurrentDateTime();
            }
        }

        void InitalizeState()
        {
            settingsData.LoadSettings();
            UIdisplayRules.Initalize();
            collectionsData.LoadCollections();
            areaServicer.StartServices();
            storylineData.Initalize();
            golemManager.Initialize();
        }

        void ReleaseState()
        {
            settingsData.SaveSettings();
            UIdisplayRules.Release();
            collectionsData.UnloadCollections();
            timeClock.SaveCurrentDateTime();
            storylineData.Release();
            golemManager.SaveGolemData();
            saveManager.TriggerSave();
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