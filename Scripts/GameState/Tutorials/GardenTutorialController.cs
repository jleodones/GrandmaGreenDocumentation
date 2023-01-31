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

            tutorialStateData.introduceFirstGolem += TriggerGolemSpawnDialogue;
            tutorialStateData.explainEvolvedGolem += TriggerGolemEvolveDialogue;
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


            tutorialStateData.introduceFirstGolem -= TriggerGolemSpawnDialogue;
            tutorialStateData.explainEvolvedGolem -= TriggerGolemEvolveDialogue;


        }

        void PlaySlideshow(SlideshowData slideshowData)
        {
            tutorialUI.SetUpSlideshow(slideshowData);
            tutorialUI.OpenUI();
        }

        #region  Core Tutorial
        void SetupGardenTutorialState()
        {
            uint progress = tutorialStateData.coreLoopTutorial.progress;

            tutorialStateData.coreLoopTutorial.storylineData.onCompletion += CleanUpCoreLoopTutorial;

            if (progress < 7)
            {
                HUD.DisableButton("cultivisionButton");
                HUD.DisableButton("collectionsButton");
                HUD.DisableButton("customizationButton");
                toolsMenu.DisableToolButton("fertilizer-container");

                areaController.onGardenTick += ForceSetTutorialPlant;

            }

            if (progress < 2)
            {
                playerInteractable.SetActive(false);

            }

            if (progress < 1)
            {
                seedUI.DeactivateUI();
            }

            if (progress == 0)
            {
                HUD.DisableButton("inventoryButton");


                playerCamera.m_Lens.OrthographicSize = 2;

                playerEntity.controller.PauseController();

                TriggerTutorialDialogue(introDialogue);
            }
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

        void DoTownSquareTap()
        {
            TriggerTutorialDialogue(leaveDialogue);
            tapHint.SetActive(true);
            tapHint.transform.position = toTownSquarePosition.position;
        }

        List<Garden.PlantState> plants;
        Vector3Int? tutorialCell = null;
        void UpdateTutorialPlant(int stage)
        {
            if (tutorialCell == null)
            {
                plants = areaController.gardenManager.GetPlants(0);
                if (plants.Count == 0)
                    return;

                tutorialCell = plants[0].cell;
            }

            areaController.DestroyPlant((Vector3Int)tutorialCell);
            areaController.CreatePlant(starterPlantID, starterPlantGenotype, (Vector3Int)tutorialCell, stage);
        }

        void ForceSetTutorialPlant()
        {
            switch (tutorialStateData.coreLoopTutorial.progress)
            {
                case 3:
                    UpdateTutorialPlant(0);
                    break;
                case 4:
                    UpdateTutorialPlant(1);
                    break;
                case 5:
                    break;
                case 6:
                    UpdateTutorialPlant(2);
                    break;
                case 7:
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

                while ((playerEntity.transform.position - hintPositions[i].position).magnitude > 2)
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


        void CleanUpCoreLoopTutorial(Storyline storyline)
        {
            areaController.onGardenTick -= ForceSetTutorialPlant;
            tutorialStateData.coreLoopTutorial.storylineData.onCompletion -= CleanUpCoreLoopTutorial;
        }
        #endregion


        #region  GolemTutorial

        void TriggerGolemSpawnDialogue()
        {
            TriggerTutorialDialogue(golemSpawnDialogue);
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
