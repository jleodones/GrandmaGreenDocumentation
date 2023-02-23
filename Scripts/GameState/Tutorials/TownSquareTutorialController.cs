using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.UI.BulletinBoard;
using GrandmaGreen.UI.Shopping;

namespace GrandmaGreen.UI.Tutorial
{
    public class TownSquareTutorialController : MonoBehaviour
    {
        public TutorialStateData tutorialStateData;
        public Core.Utilities.GameEventFlag onInteractionCompleteFlag;

        public TutorialUIDisplay tutorialUI;
        public ShopkeeperMenu shopkeeperMenu;
        public ShoppingUI shoppingUI;
        public BulletinBoardUIDisplay bulletinBoardUI;

        public string phoebeIntroNode = "Story_1";
        public string phoebeCrossbreedingNode = "Story_2";

        public TapHereUI phoebeTapHere;
        public TapHereUI bulletinBoardTapHere;
        public TapHereUI gardenTapHere;

        bool phobeInteraction = false;
        bool mailboxInteraction = false;


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

            //EventManager.instance.EVENT_INVENTORY_ADD_SEED -= CheckCrossbreedingPossible;
        }

        void PlaySlideshow(SlideshowData slideshowData)
        {
            tutorialUI.SetUpSlideshow(slideshowData);
            tutorialUI.OpenUI();
        }

        void SetupTownSquareTutorialState()
        {
            if (!tutorialStateData.coreLoopTutorial.isComplete)
            {
                phoebeTapHere.gameObject.SetActive(true);
                bulletinBoardTapHere.gameObject.SetActive(true);

                bulletinBoardUI.onPanelClosed += SetBulletinBoardInteraction;
                shoppingUI.onPanelClosed += SetPhoebeInteraction;

                shopkeeperMenu.dialogueScript.onFinishDialogue += ForceShopOpen;

                shopkeeperMenu.SetSpecialDialogue(phoebeIntroNode);
                shopkeeperMenu.m_rootVisualElement.Q<Button>("sell").SetEnabled(false);
                shopkeeperMenu.m_rootVisualElement.Q<Button>("shop").SetEnabled(false);
            }

            else if (!tutorialStateData.crossBreedingTutorial.isComplete)
            {
                shoppingUI.onPanelClosed += TriggerCrossbreedingDialogue;
            }
        }


        void SetBulletinBoardInteraction()
        {
            mailboxInteraction = true;
            bulletinBoardTapHere.gameObject.SetActive(false);

            bulletinBoardUI.onPanelClosed -= SetBulletinBoardInteraction;


            CheckTutorialState();
        }

        void ForceShopOpen()
        {
            shopkeeperMenu.OnShopTrigger();
            shopkeeperMenu.dialogueScript.onFinishDialogue -= ForceShopOpen;
        }


        void SetPhoebeInteraction()
        {
            phobeInteraction = true;
            phoebeTapHere.gameObject.SetActive(false);

            shoppingUI.onPanelClosed -= SetPhoebeInteraction;

            shopkeeperMenu.m_rootVisualElement.Q<Button>("sell").SetEnabled(true);
            shopkeeperMenu.m_rootVisualElement.Q<Button>("shop").SetEnabled(true);
            shopkeeperMenu.dialogueScript.dialogueMode = Dialogue.DialogueMode.Idle;

            CheckTutorialState();
        }

        void CheckTutorialState()
        {
            if (!phobeInteraction || !mailboxInteraction) return;

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
            tutorialStateData.onCrossbreedingFlag.Raise();
        }
    }
}
