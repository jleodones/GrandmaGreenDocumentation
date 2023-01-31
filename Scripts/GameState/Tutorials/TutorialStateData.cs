using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;

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
        public GameEventFlag onStartFlag;
        public GameEventFlag onMoveFlag;
        public GameEventFlag onInventoryFlag;
        public GameEventFlag onToolMenuFlag;
        public GameEventFlag onGardeningFlag;
        public GameEventFlag onHarvestFlag;
        public GameEventFlag onGolemSpawnedFlag;
        public GameEventFlag onGolemTalkedFlag;
        public GameEventFlag onGolemEvolvedFlag;
        public GameEventFlag onGolemTaskFlag;

        [Header("References")]
        public Entities.EntityController playerController;
        public Garden.PlayerToolData playerToolData;
        public Garden.GardenToolSet gardenToolSet;

        public event System.Action<SlideshowData> onPlaySlideshow;

        public bool AllTutorialsCompleted() => coreLoopTutorial.isComplete;

        public event System.Action enableLevelTransition;
        public event System.Action disableLevelTransition;
        public event System.Action enableInventory;

        public event System.Action enableTools;
        public event System.Action disableTools;

        public event System.Action enableTrowel;
        public event System.Action disableTrowel;

        public event System.Action tapHereMailbox;
        public event System.Action tapHereGrandma;
        public event System.Action tapHereExit;

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
        }

        public void Release()
        {
            GolemTutorialRelease();
        }

        void SetupSlideshowEvents(TutorialData tutorialData)
        {
            for (int i = 0; i < tutorialData.length; i++)
            {
                if (tutorialData.slideshowData[i] == null)
                    continue;

                int index = i;
                tutorialData.storylineData.requirements[index].onActivation += () =>
                {

                    PlaySlideshow(tutorialData.slideshowData[index]);
                };
            }
        }

        public void PlaySlideshow(SlideshowData slideshowData)
        {
            Debug.Log("Play Slideshow " + slideshowData.title);
            onPlaySlideshow?.Invoke(slideshowData);
        }


        void CoreLoopTutorialSetup()
        {
            SetupSlideshowEvents(coreLoopTutorial);

            coreLoopTutorial.storylineData.onProgress += CoreLoopTutorialProgress;
            coreLoopTutorial.storylineData.onCompletion += CoreLoopTutorialComplete;

            disableLevelTransition?.Invoke();
        }

        void CoreLoopTutorialProgress(Storyline storyline)
        {
            switch (storyline.progress)
            {
                case 0:
                    // Gramma enters garden
                    break;

                case 1:
                    //Gramma has moved to and opened letter
                    EventManager.instance.HandleEVENT_INVENTORY_ADD_SEED(1002, new Garden.Genotype("AaBb"));

                    enableInventory?.Invoke();
                    playerToolData.onSeedEquipped += onInventoryFlag.Raise;
                    break;

                case 2:
                    // Player has selected a seed

                    playerToolData.onSeedEquipped -= onInventoryFlag.Raise;
                    playerToolData.onToolSelectionStart += onToolMenuFlag.Raise;
                    enableTools?.Invoke();

                    tapHereGrandma?.Invoke();
                    break;

                case 3:
                    playerToolData.onToolSelectionStart -= onToolMenuFlag.Raise;

                    gardenToolSet.onPlant += disableTrowel;
                    gardenToolSet.onWater += onGardeningFlag.Raise;

                    break;

                case 4:
                    gardenToolSet.onPlant -= disableTrowel;
                    gardenToolSet.onWater -= onGardeningFlag.Raise;

                    //seed planted + watered, grown one stage
                    //TODO: Trigger gramma dialogue

                    playerToolData.EmptySelection();
                    enableLevelTransition?.Invoke();
                    tapHereExit?.Invoke();

                    break;

                case 5:
                    // Player has entered town square
                    disableLevelTransition?.Invoke();
                    break;

                case 6:
                    //Player has talked to phoebe, bulletin board
                    enableLevelTransition?.Invoke();
                    break;

                case 7:
                    //Player has returned to garden
                    //TODO: Force Grow plant, prevent from dying
                    gardenToolSet.onHarvest += onHarvestFlag.Raise;
                    break;
            }
        }

        void CoreLoopTutorialComplete(Storyline storyline)
        {
            gardenToolSet.onHarvest -= onHarvestFlag.Raise;

            coreLoopTutorial.storylineData.onProgress -= CoreLoopTutorialProgress;
            coreLoopTutorial.storylineData.onCompletion -= CoreLoopTutorialComplete;
        }

        System.Action<ushort, Vector3> golemSpawnedAction;
        System.Action<ushort> golemEvolvedAction;
        System.Action<int> golemTaskAction;
        void GolemTutorialSetup()
        {
            SetupSlideshowEvents(golemTutorial);

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
    }
}
