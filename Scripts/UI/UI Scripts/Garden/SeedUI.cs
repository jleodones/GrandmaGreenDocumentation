using System.Collections;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GrandmaGreen.Garden
{
    public class SeedUI : MonoBehaviour
    {
        public GardenToolSet gardenToolSet;
        public PlayerToolData playerToolData;
        public UI.Collections.TabbedInventory tabbedInventory;
        public Image seedImage;
        public Button seedButton;
        public TextMeshProUGUI textObj;
        public CanvasGroup canvasGroup;
        public Transform worldTarget;
        public RectTransform UITarget;
        public Vector2 targetOffset;
        public float smoothTime = 0.3F;
        public float maxSpeed = 1.0F;
        private Vector2 velocity = Vector2.zero;

        void Awake()
        {
            //DeactivateUI();
        }

        void OnEnable()
        {
            playerToolData.onToolSelected += CheckEquippedTool;
            playerToolData.onToolSelectionStart += CloseUI;
            playerToolData.onToolSelectionEnd += OpenUI;

            playerToolData.onSeedEquipped += SetSeed;
            playerToolData.onSeedEmpty += EmptySeed;

            gardenToolSet.onPlant += SetSeedCount;

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
            playerToolData.onToolSelected -= CheckEquippedTool;
            playerToolData.onToolSelectionStart -= CloseUI;

            playerToolData.onSeedEquipped -= SetSeed;
            playerToolData.onSeedEmpty -= EmptySeed;

            gardenToolSet.onPlant -= SetSeedCount;

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
                OpenUI();
            else
                CloseUI();
        }

        public void EnableInteraction()
        {
            seedButton.interactable = true;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public void DisableInteraction()
        {
            seedButton.interactable = false;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void OpenUI()
        {
            if (playerToolData.currentTool.toolIndex != 3)
                return;

            canvasGroup.alpha = 1;
            EnableInteraction();
            SetTarget();
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
            SetSeedCount();

        }

        void EmptySeed()
        {
            seedImage.sprite = null;
            seedImage.color = new Color(0, 0, 0, 0);

            textObj.gameObject.SetActive(false);
        }

        public void OnButtonClicked()
        {

            playerToolData.onRequireSeedEquip.Invoke();
        }

        void Update()
        {
            if (canvasGroup.alpha == 0)
                return;

            FollowTarget();
        }

        void SetSeedCount()
        {
            textObj.text = EventManager.instance.HandleEVENT_INVENTORY_GET_SEED_COUNT((ushort)playerToolData.equippedSeed, playerToolData.equippedSeedGenotype).ToString();
        }

        Vector2 targetLocation;
        void FollowTarget()
        {
            targetLocation = (Vector2)Camera.main.WorldToScreenPoint(worldTarget.position) + targetOffset;

            UITarget.anchoredPosition = Vector2.SmoothDamp(UITarget.anchoredPosition, targetLocation, ref velocity, smoothTime, maxSpeed, Time.deltaTime);
        }

        void SetTarget()
        {
            targetLocation = (Vector2)Camera.main.WorldToScreenPoint(worldTarget.position) + targetOffset;
            UITarget.anchoredPosition = targetLocation;
        }
    }
}
