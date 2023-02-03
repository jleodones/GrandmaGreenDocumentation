using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GrandmaGreen.Garden
{
    public class SeedUI : MonoBehaviour
    {
        public PlayerToolData playerToolData;
        public Collections.CollectionsSO collections;
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

            EventManager.instance.EVENT_OPEN_HUD += ActivateUI;
            EventManager.instance.EVENT_CLOSE_HUD += DeactivateUI;

            if (playerToolData.equippedSeed != 0)
                SetSeed();
        }


        void OnDisable()
        {
            //playerToolData.onToolSelected -= CheckEquippedTool;

            playerToolData.onSeedEquipped -= SetSeed;
            playerToolData.onSeedEmpty -= EmptySeed;

            EventManager.instance.EVENT_OPEN_HUD -= ActivateUI;
            EventManager.instance.EVENT_CLOSE_HUD -= DeactivateUI;

            DeactivateUI();

            ///seedButton.onClick.RemoveListener(OnButtonClicked);
        }

        void CheckEquippedTool(ToolData tool)
        {
            if (tool.toolIndex == 3)
                ActivateUI();
            else
                DeactivateUI();
        }

        public void ActivateUI()
        {
            //canvasGroup.alpha = 1;
            seedButton.interactable = true;
        }

        public void DeactivateUI()
        {
            //canvasGroup.alpha = 0;
            seedButton.interactable = false;
        }

        void SetSeed()
        {
            seedImage.sprite = collections.GetSprite(playerToolData.equippedSeed, playerToolData.equippedSeedGenotype);
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
