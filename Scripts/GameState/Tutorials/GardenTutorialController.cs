using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;
using DG.Tweening;
using Cinemachine;

namespace GrandmaGreen
{
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
        public Yarn.Unity.YarnProject introDialogue;
        public Yarn.Unity.YarnProject mailboxDialogue;

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

            areaController.onGardenTick -= ForceSetTutorialPlant;
        }

        void PlaySlideshow(SlideshowData slideshowData)
        {
            tutorialUI.SetUpSlideshow(slideshowData);
            tutorialUI.OpenUI();
        }


        void SetupGardenTutorialState()
        {
            uint progress = tutorialStateData.coreLoopTutorial.progress;

            if (progress < 7)
            {
                HUD.DisableButton("cultivisionButton");
                HUD.DisableButton("collectionsButton");
                HUD.DisableButton("customizationButton");
                toolsMenu.DisableToolButton("fertilizer-container");

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

                dialogueable.yarnProject = introDialogue;
                dialogueable.TriggerDialogue();
            }
        }

        public void OnTutorialDialogueRead()
        {
            if (dialogueable.yarnProject == introDialogue)
                ZoomCameraOut();
            else if (dialogueable.yarnProject == mailboxDialogue)
                tutorialStateData.onMoveFlag.Raise();
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
            tapHint.SetActive(true);
            tapHint.transform.position = toTownSquarePosition.position;
        }

        List<Garden.PlantState> plants;
        void UpdateTutorialPlant(int stage)
        {
            plants = areaController.gardenManager.GetPlants(0);
            if (plants.Count == 0)
                return;

            Vector3Int cell = plants[0].cell;

            areaController.DestroyPlant(cell);
            areaController.CreatePlant(starterPlantID, starterPlantGenotype, cell, stage);
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

            dialogueable.yarnProject = mailboxDialogue;
            dialogueable.TriggerDialogue();
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
    }
}
