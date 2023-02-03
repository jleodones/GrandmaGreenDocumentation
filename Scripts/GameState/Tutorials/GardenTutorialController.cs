using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;
using DG.Tweening;
using Cinemachine;

namespace GrandmaGreen
{
    [System.Serializable]
    public class DialoguePair
    {
        public Yarn.Unity.YarnProject yarnProject;
        public string node;
    }

    public class GardenTutorialController : MonoBehaviour
    {
        [Header("Data References")]
        public TutorialStateData tutorialStateData;
        public Collections.PlantId starterPlantID;
        public Garden.Genotype starterPlantGenotype;

        [Header("UI References")]
        public Garden.GardenAreaController areaController;
        public ToolsMenu toolsMenu;
        public UI.HUD.HUD HUD;
        public Garden.SeedUI seedUI;
        public TutorialUIDisplay tutorialUI;
        public Dialogue.Dialogueable dialogueable;
        public DialoguePair introDialogue;
        public DialoguePair mailboxDialogue;
        public DialoguePair leaveDialogue;
        public DialoguePair golemSpawnDialogue;
        public DialoguePair golemEvolveDialogue;

        [Header("Scene References")]
        public Entities.GameEntity playerEntity;
        public GameObject playerInteractable;
        public GameObject mailbox;
        public GameObject tapHint;
        public CinemachineVirtualCamera playerCamera;
        public Transform[] hintPositions;
        public Transform toTownSquarePosition;
        public Collider mailboxColiider;

        void Awake()
        {
            if (tutorialStateData.AllTutorialsCompleted() || !tutorialStateData.tutorialEnabled)
                Destroy(gameObject);
        }

        void Start()
        {
            SetupGardenTutorialState();
            ForceSetTutorialPlant();
        }

        void OnEnable()
        {
            tutorialStateData.enableInventory += EnableInventory;
            tutorialStateData.enableTools += EnableTools;
            tutorialStateData.disableTools += DisableTools;
            tutorialStateData.disableTrowel += DisableTrowel;
            tutorialStateData.onPlaySlideshow += PlaySlideshow;
            tutorialStateData.tapHereGrandma += DoGrandmaTap;
            tutorialStateData.tapHereExit += DoTownSquareTap;

            tutorialStateData.disableSeedPacket += DisableSeeds;
            tutorialStateData.enableSeedPacket += EnableSeeds;
            tutorialStateData.disableWatering += DisableWatering;
            tutorialStateData.enableWatering += EnableWatering;


            tutorialStateData.introduceFirstGolem += TriggerGolemSpawnDialogue;
            tutorialStateData.explainEvolvedGolem += TriggerGolemEvolveDialogue;

            areaController.onGardenTick += ForceSetTutorialPlant;
        }


        void OnDisable()
        {
            tutorialStateData.enableInventory -= EnableInventory;
            tutorialStateData.enableTools -= EnableTools;
            tutorialStateData.disableTools -= DisableTools;
            tutorialStateData.disableTrowel -= DisableTrowel;
            tutorialStateData.onPlaySlideshow -= PlaySlideshow;
            tutorialStateData.tapHereGrandma -= DoGrandmaTap;
            tutorialStateData.tapHereExit -= DoTownSquareTap;

            tutorialStateData.disableSeedPacket -= DisableSeeds;
            tutorialStateData.enableSeedPacket -= EnableSeeds;
            tutorialStateData.disableWatering -= DisableWatering;
            tutorialStateData.enableWatering -= EnableWatering;


            tutorialStateData.introduceFirstGolem -= TriggerGolemSpawnDialogue;
            tutorialStateData.explainEvolvedGolem -= TriggerGolemEvolveDialogue;

            areaController.onGardenTick -= ForceSetTutorialPlant;
        }

        void PlaySlideshow(SlideshowData slideshowData)
        {
            tutorialUI.SetUpSlideshow(slideshowData);
            tutorialUI.OpenUI();
        }

        #region  Core Tutorial
        void SetupGardenTutorialState()
        {
            if (!tutorialStateData.crossBreedingTutorial.isComplete && tutorialStateData.crossBreedingTutorial.progress <= 1)
                HUD.DisableButton("cultivisionButton");

            if (!tutorialStateData.golemTutorial.isComplete && tutorialStateData.golemTutorial.progress < 1)
                toolsMenu.DisableToolButton("fertilizer-container");

            uint progress = tutorialStateData.coreLoopTutorial.progress;

            if (progress < 9)
            {
                HUD.DisableButton("collectionsButton");
                HUD.DisableButton("customizationButton");
                mailboxColiider.enabled = false;
            }

            if (progress < 2)
            {
                playerInteractable.SetActive(false);
            }

            if (progress < 1)
            {
                seedUI.enabled = false;
            }

            if (progress == 0)
            {
                HUD.DisableButton("inventoryButton");

                playerCamera.m_Lens.OrthographicSize = 2;

                playerEntity.controller.PauseController();

                TriggerTutorialDialogue(introDialogue);
            }

            if (progress > 4)
                areaController.onGardenTick += ForceSetTutorialPlant;
        }

        public void OnTutorialDialogueRead()
        {
            if (lastDialogue == introDialogue && !tutorialStateData.coreLoopTutorial.isComplete)
                ZoomCameraOut();
            else if (lastDialogue == mailboxDialogue && !tutorialStateData.coreLoopTutorial.isComplete)
                tutorialStateData.onMoveFlag.Raise();
            else if (lastDialogue == golemSpawnDialogue || lastDialogue == golemEvolveDialogue)
                tutorialStateData.onGolemTalkedFlag.Raise();
        }

        void ZoomCameraOut()
        {
            DOTween.To(() => playerCamera.m_Lens.OrthographicSize, (x) => playerCamera.m_Lens.OrthographicSize = x, 5, 2.0f)
                .OnComplete(
                    () =>
                    {
                        tutorialStateData.PlaySlideshow(tutorialStateData.coreLoopTutorial.slideshowData[0]);
                        WaitForPlayerToMove().Start();
                        playerEntity.controller.StartController();
                    }
                    );
        }

        void DoGrandmaTap()
        {
            StartCoroutine(WaitForGrandmaTap());
        }

        void EnableInventory()
        {
            HUD.EnableButton("inventoryButton");
            seedUI.enabled = true;
            seedUI.ActivateUI();
        }

        void EnableTools()
        {
            playerInteractable.SetActive(true);
        }

        void DisableTools()
        {
            playerInteractable.SetActive(false);
        }

        void DisableTrowel()
        {
            toolsMenu.DisableToolButton("trowel-container");
        }

        void EnableSeeds()
        {
            toolsMenu.EnableToolButton("seeds-container");
        }

        void DisableSeeds()
        {
            toolsMenu.DisableToolButton("seeds-container");
        }

        void EnableWatering()
        {
            toolsMenu.EnableToolButton("watering-container");
        }

        void DisableWatering()
        {
            toolsMenu.DisableToolButton("watering-container");
        }

        void DoTownSquareTap()
        {
            TriggerTutorialDialogue(leaveDialogue);
            tapHint.SetActive(true);
            tapHint.transform.position = toTownSquarePosition.position;
        }

        List<Garden.PlantState> plants;
        Vector3Int tutorialCell;
        void UpdateTutorialPlant(int stage)
        {
            plants = areaController.gardenManager.GetPlants(0);
            if (plants.Count == 0)
                return;

            tutorialCell = plants[0].cell;

            areaController.DestroyPlant(tutorialCell);
            areaController.CreatePlant(starterPlantID, starterPlantGenotype, tutorialCell, stage);
        }

        void ForceSetTutorialPlant()
        {
            switch (tutorialStateData.coreLoopTutorial.progress)
            {
                case 5:
                    UpdateTutorialPlant(0);
                    break;
                case 6:
                    UpdateTutorialPlant(1);
                    break;
                case 9:
                    UpdateTutorialPlant(2);
                    break;
            }
        }

        int currentHintPos = 0;
        IEnumerator WaitForPlayerToMove()
        {
            tapHint.SetActive(true);
            tapHint.transform.position = hintPositions[0].position;

            for (int i = 0; i < hintPositions.Length; i++)
            {

                while ((playerEntity.transform.position - hintPositions[i].position).magnitude > 2.5f)
                    yield return null;

                if (i < hintPositions.Length - 1)
                    tapHint.transform.DOMove(hintPositions[i + 1].position, 1.0f);
            }

            tapHint.SetActive(false);

            TriggerTutorialDialogue(mailboxDialogue);
        }

        IEnumerator WaitForGrandmaTap()
        {
            tapHint.SetActive(true);

            bool grandmaTapped = false;

            System.Action onGrandmaTapped = () => grandmaTapped = true;
            tutorialStateData.playerToolData.onToolSelectionStart += onGrandmaTapped;

            while (!grandmaTapped)
            {
                tapHint.transform.position = playerEntity.transform.position;
                yield return null;
            }


            tutorialStateData.playerToolData.onToolSelectionStart -= onGrandmaTapped;

            tapHint.SetActive(false);
        }


        #endregion


        #region  GolemTutorial

        void TriggerGolemSpawnDialogue()
        {
            TriggerTutorialDialogue(golemSpawnDialogue);
            toolsMenu.DisableToolButton("fertilizer-container");
        }

        void TriggerGolemEvolveDialogue()
        {
            TriggerTutorialDialogue(golemEvolveDialogue);
        }

        #endregion


        DialoguePair lastDialogue;
        void TriggerTutorialDialogue(DialoguePair dialoguePair)
        {
            lastDialogue = dialoguePair;
            dialogueable.yarnProject = lastDialogue.yarnProject;
            dialogueable.TriggerDialogueByNode(lastDialogue.node);
        }
    }
}
