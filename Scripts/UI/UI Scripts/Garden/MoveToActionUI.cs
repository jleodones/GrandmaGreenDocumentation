using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using GrandmaGreen.Entities;
using DG.Tweening;

namespace GrandmaGreen.Garden
{
    public class MoveToActionUI : MonoBehaviour
    {
        public PlayerController playerController;
        public PlayerToolData toolData;
        public AreaServices areaServices;
        public GameObject iconUIObject;
        public GameObject tileHighlightUIObject;
        public UnityEngine.UI.Image actionIcon;

        UIDocument uiDoc;
        VisualElement root;

        // Start is called before the first frame update
        void Start()
        {
            startPos = actionIcon.transform.localPosition;

            playerController.entity.onEntityPathStart += StartMoveToAction;
            playerController.entity.onEntityPathEnd += EndMoveToAction;

            tween = actionIcon.transform.DOBlendableLocalMoveBy(Vector3.up * 50.0f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            tween.Pause();
        }
        Vector3 startPos;
        Tween tween;
        Coroutine routine;
        void StartMoveToAction()
        {
            if (routine != null)
                StopCoroutine(routine);

            if (!tween.IsPlaying())
                tween.Play();

            iconUIObject.SetActive(true);
            tileHighlightUIObject.SetActive(true);

            if (toolData.currentTool.icon == null)
            {
                iconUIObject.gameObject.SetActive(false);
            }
            else
            {
                actionIcon.sprite = toolData.currentTool.icon;
                iconUIObject.gameObject.SetActive(true);
            }

            Vector3 goalPos = areaServices.ActiveArea.tilemap.CellToWorld(areaServices.ActiveArea.lastSelectedCell) + (Vector3)(Vector2.one * 0.5f);

            (iconUIObject.transform as RectTransform).anchoredPosition = Camera.main.WorldToScreenPoint(goalPos);
            tileHighlightUIObject.transform.position = goalPos;
            routine = StartCoroutine(SetUIPosition(goalPos));
        }

        IEnumerator SetUIPosition(Vector3 goalPos)
        {
            while (iconUIObject.activeSelf)
            {
                (iconUIObject.transform as RectTransform).anchoredPosition = Camera.main.WorldToScreenPoint(goalPos);
                yield return null;
            }
        }

        void EndMoveToAction()
        {
            tween.Pause();
            actionIcon.transform.localPosition = startPos;
            iconUIObject.SetActive(false);
            tileHighlightUIObject.SetActive(false);
        }
    }
}
