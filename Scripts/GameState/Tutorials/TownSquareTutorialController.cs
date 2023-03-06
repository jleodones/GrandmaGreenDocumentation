using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.UI.BulletinBoard;
using System.Collections;
using System.Collections.Generic;
using GrandmaGreen.UI.Shopping;
using Cinemachine;

namespace GrandmaGreen.UI.Tutorial
{
    public class TownSquareTutorialController : MonoBehaviour
    {
        public TutorialStateData tutorialStateData;
        public Core.Utilities.GameEventFlag onInteractionCompleteFlag;

        [Header("Scene References")]
        public Entities.GameEntity playerEntity;
        public TutorialUIDisplay tutorialUI;
        public ShopkeeperMenu shopkeeperMenu;
        public ShoppingUI shoppingUI;
        public BulletinBoardUIDisplay bulletinBoardUI;
        public Transform walkToPosition;
        public CinemachineVirtualCamera walkToCamera;

        [Header("Tutorial Settings")]
        public string phoebeIntroNode = "Story_1";
        public string phoebeCrossbreedingNode = "Story_2";

        public TapHereUI phoebeTapHere;
        public TapHereUI bulletinBoardTapHere;
        public TapHereUI gardenTapHere;

        bool phobeInteraction = false;
        bool bulletinBoardInteraction = false;


        void Awake()
        {
            if (tutorialStateData.AllTutorialsCompleted() || !tutorialStateData.tutorialEnabled)
                Destroy(gameObject);
        }

        void Start()
        {
            SetupTownSquareTutorialState();
        }

        void OnEnable()
        {
            tutorialStateData.onPlaySlideshow += PlaySlideshow;
        }

        void OnDisable()
        {
            tutorialStateData.onPlaySlideshow -= PlaySlideshow;
        }

        System.Action onSlideshowComplete;
        void PlaySlideshow(TutorialSlideshow tutorialSlideshow)
        {
            tutorialUI.SetUpSlideshow(tutorialSlideshow.slideshow);
            tutorialUI.OpenUI();

            onSlideshowComplete = () => tutorialSlideshow.flag?.Raise();

            tutorialUI.onPanelClosed += SlideshowComplete;

        }

        void SlideshowComplete()
        {
            onSlideshowComplete?.Invoke();
            tutorialUI.onPanelClosed -= SlideshowComplete;
        }

        void SetupTownSquareTutorialState()
        {
            //tutorialStateData.NextCoreLoopTutorial();

            if (!onInteractionCompleteFlag.raised)
            {

                bulletinBoardUI.onPanelClosed += SetBulletinBoardInteraction;
                shoppingUI.onPanelClosed += SetPhoebeInteraction;

                shopkeeperMenu.dialogueScript.onFinishDialogue += ForceShopOpen;

                shopkeeperMenu.onPanelOpened += StartPhoebeInteraction;
                bulletinBoardUI.onPanelOpened += StartBulletinBoardInteraction;

                shopkeeperMenu.SetSpecialDialogue(phoebeIntroNode);
                shopkeeperMenu.m_rootVisualElement.Q<Button>("sell").SetEnabled(false);
                shopkeeperMenu.m_rootVisualElement.Q<Button>("shop").SetEnabled(false);

                StartCoroutine(EnterTownSquareSequence());
            }

            else if (!tutorialStateData.crossBreedingTutorial.isComplete)
            {
                shoppingUI.onPanelClosed += TriggerCrossbreedingDialogue;
            }
        }

        public IEnumerator EnterTownSquareSequence()
        {
            playerEntity.controller.PauseController();

            yield return new WaitForSeconds(0.5f);

            walkToCamera.Priority++;

            yield return new WaitForSeconds(2.0f);

            yield return playerEntity.FollowPath(playerEntity.CheckPath(walkToPosition.position));

            yield return new WaitForSeconds(2.0f);

            walkToCamera.Priority--;
            playerEntity.controller.StartController();
            tutorialStateData.NextCoreLoopTutorial();

            phoebeTapHere.gameObject.SetActive(true);
            bulletinBoardTapHere.gameObject.SetActive(true);
        }

        void StartBulletinBoardInteraction()
        {
            bulletinBoardTapHere.gameObject.SetActive(false);
            bulletinBoardUI.onPanelOpened -= StartBulletinBoardInteraction;
        }
        void SetBulletinBoardInteraction()
        {
            bulletinBoardInteraction = true;


            bulletinBoardUI.onPanelClosed -= SetBulletinBoardInteraction;


            CheckTutorialState();
        }

        void ForceShopOpen()
        {
            shopkeeperMenu.OnShopTrigger();
            shopkeeperMenu.dialogueScript.onFinishDialogue -= ForceShopOpen;
        }

        void StartPhoebeInteraction()
        {
            phoebeTapHere.gameObject.SetActive(false);
            shopkeeperMenu.onPanelOpened -= StartPhoebeInteraction;

        }
        void SetPhoebeInteraction()
        {
            phobeInteraction = true;

            shoppingUI.onPanelClosed -= SetPhoebeInteraction;
            shopkeeperMenu.onPanelOpened -= StartPhoebeInteraction;

            shopkeeperMenu.m_rootVisualElement.Q<Button>("sell").SetEnabled(true);
            shopkeeperMenu.m_rootVisualElement.Q<Button>("shop").SetEnabled(true);
            shopkeeperMenu.dialogueScript.dialogueMode = Dialogue.DialogueMode.Idle;

            CheckTutorialState();
        }

        void CheckTutorialState()
        {
            if (!phobeInteraction || !bulletinBoardInteraction) return;

            onInteractionCompleteFlag.Raise();

            gardenTapHere.gameObject.SetActive(true);
        }


        void TriggerCrossbreedingDialogue()
        {
            shoppingUI.onPanelClosed -= TriggerCrossbreedingDialogue;
            shopkeeperMenu.SetSpecialDialogue(phoebeCrossbreedingNode);

            shopkeeperMenu.dialogueScript.onFinishDialogue += SetCrossbreedingDialogueComplete;
            shopkeeperMenu.OnDialogueTrigger();
        }

        void SetCrossbreedingDialogueComplete()
        {
            shopkeeperMenu.dialogueScript.onFinishDialogue -= SetCrossbreedingDialogueComplete;

            tutorialStateData.PlayNextSlideshow(tutorialStateData.crossBreedingTutorial);
        }
    }
}
