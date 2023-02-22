using System.Collections;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GrandmaGreen.Garden
{
    public class SeedUI : MonoBehaviour
    {
        public PlayerToolData playerToolData;
        public UI.Collections.TabbedInventory tabbedInventory;
        public Image seedImage;
        public Button seedButton;
        public CanvasGroup canvasGroup;

        void Awake()
        {
            //DeactivateUI();
        }

        void OnEnable()
        {
            //playerToolData.onToolSelected += CheckEquippedTool;

            playerToolData.onSeedEquipped += SetSeed;
            playerToolData.onSeedEmpty += EmptySeed;

            //seedButton.onClick.AddListener(OnButtonClicked);

            EventManager.instance.EVENT_OPEN_HUD += OpenUI;
            EventManager.instance.EVENT_CLOSE_HUD += CloseUI;

            EventManager.instance.EVENT_TOGGLE_CUSTOMIZATION_MODE += ToggleUI;


            tabbedInventory.onPanelOpened += OpenUI;
            tabbedInventory.onPanelOpened += DisableInteraction;

            tabbedInventory.onPanelClosed += EnableInteraction;

            if (playerToolData.equippedSeed != 0)
                SetSeed();

            OpenUI();
        }


        void OnDisable()
        {
            //playerToolData.onToolSelected -= CheckEquippedTool;

            playerToolData.onSeedEquipped -= SetSeed;
            playerToolData.onSeedEmpty -= EmptySeed;

            EventManager.instance.EVENT_OPEN_HUD -= OpenUI;
            EventManager.instance.EVENT_CLOSE_HUD -= CloseUI;

            EventManager.instance.EVENT_TOGGLE_CUSTOMIZATION_MODE -= ToggleUI;

            tabbedInventory.onPanelOpened -= OpenUI;
            tabbedInventory.onPanelOpened -= DisableInteraction;

            tabbedInventory.onPanelClosed -= EnableInteraction;


            CloseUI();

            ///seedButton.onClick.RemoveListener(OnButtonClicked);
        }

        void CheckEquippedTool(ToolData tool)
        {
            if (tool.toolIndex == 3)
                EnableInteraction();
            else
                DisableInteraction();
        }

        public void EnableInteraction()
        {
            //canvasGroup.alpha = 1;
            seedButton.interactable = true;
        }

        public void DisableInteraction()
        {
            //canvasGroup.alpha = 0;
            seedButton.interactable = false;
        }

        public void OpenUI()
        {
            canvasGroup.alpha = 1;
            EnableInteraction();
        }

        public void CloseUI()
        {
            canvasGroup.alpha = 0;
            DisableInteraction();
        }

        void ToggleUI()
        {
            if (canvasGroup.alpha == 0)
                OpenUI();
            else
                CloseUI();
        }

        void SetSeed()
        {
            seedImage.sprite = CollectionsSO.LoadedInstance.GetSprite(playerToolData.equippedSeed, playerToolData.equippedSeedGenotype);
            seedImage.color = Color.white;
        }

        void EmptySeed()
        {
            seedImage.sprite = null;
            seedImage.color = new Color(0, 0, 0, 0);
        }

        public void OnButtonClicked()
        {

            playerToolData.onRequireSeedEquip.Invoke();
        }
    }
}
