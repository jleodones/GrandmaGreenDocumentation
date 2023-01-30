using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen
{
    public class TownSquareTutorialController : MonoBehaviour
    {
        public TutorialStateData tutorialStateData;
        public Core.Utilities.GameEventFlag onInteractionCompleteFlag;
        public UI.Shopping.ShoppingUI shoppingUI;
        public BulletinBoardUIDisplay bulletinBoardUI;

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

        void SetupTownSquareTutorialState()
        {
            phoebeTapHere.gameObject.SetActive(true);
            bulletinBoardTapHere.gameObject.SetActive(true);

            bulletinBoardUI.onPanelClosed += SetBulletinBoardInteraction;
            shoppingUI.onPanelClosed += SetPhoebeInteraction;
        }

        void SetBulletinBoardInteraction()
        {
            mailboxInteraction = true;
            bulletinBoardTapHere.gameObject.SetActive(false);

            bulletinBoardUI.onPanelClosed -= SetBulletinBoardInteraction;


            CheckTutorialState();
        }

        void SetPhoebeInteraction()
        {
            phobeInteraction = true;
            phoebeTapHere.gameObject.SetActive(false);

            shoppingUI.onPanelClosed -= SetPhoebeInteraction;

            CheckTutorialState();
        }

        void CheckTutorialState()
        {
            if (!phobeInteraction || !mailboxInteraction) return;

            onInteractionCompleteFlag.Raise();

            gardenTapHere.gameObject.SetActive(true);
        }
    }
}
