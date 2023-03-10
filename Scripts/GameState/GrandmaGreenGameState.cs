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
        [SerializeField] GrandmaGreenLevelLoader levelLoader;
        [SerializeField] TutorialStateData tutorialStateData;
        [SerializeField] CollectionsSO collectionsData;
        [SerializeField] AreaServices areaServicer;
        [SerializeField] TimeLayerClock timeClock;
        [SerializeField] StorylineDataStore storylineData;
        [SerializeField] SaveSystem.SaveManager saveManager;
        [SerializeField] Entities.GolemManager golemManager;
        [SerializeField] Garden.PlayerToolData playerToolData;
        [SerializeField] AchivementDataStore achivementData;
        [SerializeField] UI.UIDisplayTracker UIdisplayRules;
        [SerializeField] SCENES currentScene;
        [SerializeField] Core.Utilities.GameEventFlag onLevelTransitionFlag;
        [ReadOnly] int activeAreaIndex;
        [ReadOnly] bool isPaused;
        [ReadOnly] bool levelTransitionEnabled = true;

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
        
            if (s_Instance != this)
                return;

            ReleaseState();

            areaServicer.StopServices();

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
            tutorialStateData.enableLevelTransition += EnableSceneTransition;
            tutorialStateData.disableLevelTransition += DisableSceneTransition;

            settingsData.LoadSettings();
            UIdisplayRules.Initalize();
            collectionsData.LoadCollections();
            areaServicer.StartServices();
            storylineData.Initalize();
            golemManager.Initialize();
            playerToolData.ClearTools();
            tutorialStateData.Initalize();
            achivementData.Initalize();

            levelLoader.asyncLoadReq += GameSceneTransition;

            EventManager.instance.EVENT_DELETE_SAVE += playerToolData.ClearTools;
        }

        void ReleaseState()
        {
            tutorialStateData.enableLevelTransition -= EnableSceneTransition;
            tutorialStateData.disableLevelTransition -= DisableSceneTransition;

            settingsData.SaveSettings();
            UIdisplayRules.Release();
            collectionsData.UnloadCollections();
            timeClock.SaveCurrentDateTime();
            storylineData.Release();
            golemManager.SaveGolemData();
            achivementData.Release();

            saveManager.TriggerSave();


            tutorialStateData.Release();

            levelLoader.asyncLoadReq -= GameSceneTransition;

            EventManager.instance.EVENT_DELETE_SAVE -= playerToolData.ClearTools;
        }




        void SetCurrentScene(SCENES scene)
        {
            if (scene != SCENES.StartScene)
            {
                areaServicer.ActivateAreaController(0);
                currentScene = scene;
            }
        }

        bool inSceneTransition = false;
        public void GameSceneTransition(SCENES scene)
        {
            if (!levelTransitionEnabled || inSceneTransition)
                return;

            StartSceneTransition(scene);

        }

        void StartSceneTransition(SCENES scene)
        {
            SceneHandler.onSceneLoaded += FinishSceneTransition;
            scene.LoadAsync();
        }

        void FinishSceneTransition(SCENES scene)
        {
            SceneHandler.onSceneLoaded -= FinishSceneTransition;
            onLevelTransitionFlag.Raise();

        }

        void EnableSceneTransition()
        {
            levelTransitionEnabled = true;
        }

        void DisableSceneTransition()
        {
            levelTransitionEnabled = false;
        }


        void InitalizePlayerTools()
        {

        }
    }
}