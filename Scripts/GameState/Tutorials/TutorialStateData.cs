using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace GrandmaGreen
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Tutorials/TutorialStateData ")]
    public class TutorialStateData : ScriptableObject
    {
        [Header("Tutorials")]
        public bool tutorialEnabled = true;
        public TutorialData coreLoopTutorial;
        public TutorialData golemTutorial;
        public TutorialData crossBreedingTutorial;

        [Header("Flag Set")]
        public GameEventFlag onGolemSpawnedFlag;
        public GameEventFlag onGolemTalkedFlag;
        public GameEventFlag onGolemEvolvedFlag;
        public GameEventFlag onGolemTaskFlag;
        public GameEventFlag onCrossbreedingFlag;
        public GameEventFlag onCultivisionFlag;

        [Header("References")]
        public Entities.EntityController playerController;
        public Garden.PlayerToolData playerToolData;
        public Garden.GardenToolSet gardenToolSet;

        public event System.Action<TutorialSlideshow> onPlaySlideshow;

        public bool AllTutorialsCompleted() => coreLoopTutorial.isComplete && golemTutorial.isComplete && crossBreedingTutorial.isComplete;

        public event System.Action startGrandmaMoveTo;

        public event System.Action enableLevelTransition;
        public event System.Action disableLevelTransition;
        public event System.Action enableInventory;

        public event System.Action enableTools;
        public event System.Action disableTools;

        public event System.Action enableTrowel;
        public event System.Action disableTrowel;

        public event System.Action enableSeedPacket;
        public event System.Action disableSeedPacket;

        public event System.Action enableWatering;
        public event System.Action disableWatering;

        public event System.Action tapHereMailbox;
        public event System.Action tapHereGrandma;
        public event System.Action tapHereExit;

        public event System.Action unlockExtraUI;

        public event System.Action introduceFirstGolem;
        public event System.Action explainEvolvedGolem;

        public void Initalize()
        {
            if (!tutorialEnabled)
                return;

            if (AllTutorialsCompleted())
            {
                coreLoopTutorial = null;
                golemTutorial = null;
                crossBreedingTutorial = null;
                return;
            }

            if (!coreLoopTutorial.isComplete)
                CoreLoopTutorialSetup();

            if (!golemTutorial.isComplete)
                GolemTutorialSetup();

            if (!crossBreedingTutorial.isComplete)
                CrossbreedingTutorialSetup();
        }

        public void Release()
        {
            GolemTutorialRelease();
        }

        void SetupSlideshowEvents(TutorialData tutorialData)
        {
            for (int i = 0; i < tutorialData.length; i++)
            {

                int index = i;
                tutorialData.storylineData.requirements[index].onCompletion += () =>
                {
                    if (tutorialData.slideshowData[index].slideshow == null)
                    {
                        tutorialData.slideshowData[index].flag?.Raise();
                        return;
                    }
                    DisableEventSystem();

                    DOTween.Sequence().AppendInterval(tutorialData.slideshowData[index].openDelay)
                    .AppendCallback(() =>
                    {
                        PlaySlideshow(tutorialData.slideshowData[index]);
                        EnableEventSystem();
                    }
                    );
                };
            }
        }

        EventSystem eventSystem;
        void DisableEventSystem()
        {
            eventSystem = EventSystem.current;
            eventSystem.enabled = false;
        }

        void EnableEventSystem()
        {
            eventSystem.enabled = true;
        }

        public void PlaySlideshow(TutorialSlideshow slideshowData)
        {
            onPlaySlideshow?.Invoke(slideshowData);
        }

        public void PlayNextSlideshow(TutorialData tutorialData)
        {
            int index = (int)tutorialData.progress;

            if (tutorialData.slideshowData[index].slideshow == null)
            {
                tutorialData.slideshowData[index].flag?.Raise();
                return;
            }
            DisableEventSystem();

            DOTween.Sequence().AppendInterval(tutorialData.slideshowData[index].openDelay)
            .AppendCallback(() =>
            {
                EnableEventSystem();
                onPlaySlideshow?.Invoke(tutorialData.slideshowData[index]);
            }
           );
        }

        #region  Coreloop
        void CoreLoopTutorialSetup()
        {
            //SetupSlideshowEvents(coreLoopTutorial);

            coreLoopTutorial.storylineData.onProgress += CoreLoopTutorialProgress;
            coreLoopTutorial.storylineData.onCompletion += CoreLoopTutorialComplete;

            disableLevelTransition?.Invoke();
        }

        public void NextCoreLoopTutorial()
        {
            PlayNextSlideshow(coreLoopTutorial);
        }

        public void CoreLoopTutorialProgress(Storyline storyline)
        {
            switch (storyline.progress)
            {
                case 0:
                    // Gramma enters garden
                    break;
                case 1:
                    // Gramma enters garden
                    startGrandmaMoveTo?.Invoke();
                    break;

                case 2:
                    //Gramma has moved to and opened letter
                    EventManager.instance.HandleEVENT_OPEN_HUD();
                    EventManager.instance.HandleEVENT_INVENTORY_ADD_SEED(1002, new Garden.Genotype("AaBb"));


                    enableInventory?.Invoke();
                    playerToolData.onSeedEquipped += NextCoreLoopTutorial;
                    break;

                case 3:
                    // Player has selected a seed

                    playerToolData.onSeedEquipped -= NextCoreLoopTutorial;
                    playerToolData.onToolSelectionStart += NextCoreLoopTutorial;
                    enableTools?.Invoke();

                    tapHereGrandma?.Invoke();
                    break;

                case 4:
                    playerToolData.onToolSelectionStart -= NextCoreLoopTutorial;
                    gardenToolSet.onTill += NextCoreLoopTutorial;
                    break;

                case 5:

                    enableSeedPacket?.Invoke();

                    gardenToolSet.onTill -= NextCoreLoopTutorial;
                    gardenToolSet.onPlant += NextCoreLoopTutorial;
                    break;
                case 6:
                    disableTrowel?.Invoke();
                    //disableSeedPacket?.Invoke();
                    //enableWatering?.Invoke();

                    gardenToolSet.onPlant -= NextCoreLoopTutorial;
                    gardenToolSet.onWater += NextCoreLoopTutorial;
                    break;
                case 7:
                    gardenToolSet.onWater -= NextCoreLoopTutorial;

                    playerToolData.EmptySelection();
                    enableLevelTransition?.Invoke();
                    tapHereExit?.Invoke();
                    break;
                case 8:
                    // Player has entered town square
                    disableLevelTransition?.Invoke();

                    break;

                case 10:
                    //Player has talked to phoebe, bulletin board
                    enableLevelTransition?.Invoke();
                    break;

                case 11:
                    NextCoreLoopTutorial();
                    //Player has returned to garden
                    //TODO: Force Grow plant, prevent from dying
                    gardenToolSet.onHarvest += NextCoreLoopTutorial;
                    disableSeedPacket?.Invoke();
                    //disableWatering?.Invoke();
                    disableLevelTransition?.Invoke();
                    break;
            }
        }

        void CoreLoopTutorialComplete(Storyline storyline)
        {
            gardenToolSet.onHarvest -= NextCoreLoopTutorial;

            coreLoopTutorial.storylineData.onProgress -= CoreLoopTutorialProgress;
            coreLoopTutorial.storylineData.onCompletion -= CoreLoopTutorialComplete;

            enableSeedPacket?.Invoke();
            enableWatering?.Invoke();
            enableLevelTransition?.Invoke();
            unlockExtraUI?.Invoke();
        }

        #endregion

        #region  Golems

        System.Action<ushort, Vector3> golemSpawnedAction;
        System.Action<ushort> golemEvolvedAction;
        System.Action<int> golemTaskAction;
        void GolemTutorialSetup()
        {
            //SetupSlideshowEvents(golemTutorial);

            golemTutorial.storylineData.onProgress += GolemTutorialProgress;
            golemTutorial.storylineData.onCompletion += GolemTutorialComplete;

            if (golemTutorial.progress == 0)
            {
                golemSpawnedAction = (_, _) => onGolemSpawnedFlag.Raise();
                EventManager.instance.EVENT_GOLEM_SPAWN += golemSpawnedAction;
            }
            else if (golemTutorial.progress == 2)
            {
                golemEvolvedAction = (_) => onGolemEvolvedFlag.Raise();
                EventManager.instance.EVENT_GOLEM_EVOLVE += golemEvolvedAction;
            }
            else if (golemTutorial.progress == 4)
            {
                golemTaskAction = (_) => onGolemTaskFlag.Raise();
                EventManager.instance.EVENT_GOLEM_DO_TASK += golemTaskAction;
            }
        }

        void GolemTutorialRelease()
        {
            EventManager.instance.EVENT_GOLEM_SPAWN -= golemSpawnedAction;
            EventManager.instance.EVENT_GOLEM_EVOLVE -= golemEvolvedAction;
            EventManager.instance.EVENT_GOLEM_DO_TASK -= golemTaskAction;
        }

        void GolemTutorialProgress(Storyline storyline)
        {
            switch (storyline.progress)
            {
                case 1:
                    introduceFirstGolem?.Invoke();
                    break;
                case 2:
                    golemEvolvedAction = (_) => onGolemEvolvedFlag.Raise();
                    EventManager.instance.EVENT_GOLEM_EVOLVE += golemEvolvedAction;
                    break;
                case 3:
                    explainEvolvedGolem?.Invoke();

                    break;
                case 4:
                    golemTaskAction = (_) => onGolemTaskFlag.Raise();
                    EventManager.instance.EVENT_GOLEM_DO_TASK += golemTaskAction;
                    break;
            }
        }

        void GolemTutorialComplete(Storyline storyline)
        {
            golemTutorial.storylineData.onProgress -= GolemTutorialProgress;
            golemTutorial.storylineData.onCompletion -= GolemTutorialComplete;
        }
        #endregion

        #region Crossbreeding
        void CrossbreedingTutorialSetup()
        {
            //SetupSlideshowEvents(crossBreedingTutorial);

            crossBreedingTutorial.storylineData.onProgress += CrossbreedingTutorialProgress;
            crossBreedingTutorial.storylineData.onCompletion += CrossbreedingTutorialComplete;

        }

        void CrossbreedingTutorialProgress(Storyline storyline)
        {
            switch (storyline.progress)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    PlayNextSlideshow(crossBreedingTutorial);
                    break;
            }
        }

        void CrossbreedingTutorialComplete(Storyline storyline)
        {
            crossBreedingTutorial.storylineData.onProgress -= CrossbreedingTutorialProgress;
            crossBreedingTutorial.storylineData.onCompletion -= CrossbreedingTutorialComplete;
        }

        #endregion
    }
}
