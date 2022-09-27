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

        public PlayerToolData toolData;
        public GameObject moveToActionUIObject;
        public UnityEngine.UI.Image actionIcon;

        UIDocument uiDoc;
        VisualElement root;

        // Start is called before the first frame update
        void Start()
        {
            //uiDoc = GetComponent<UIDocument>();
            //root = uiDoc.rootVisualElement;
            startPos = actionIcon.transform.localPosition;
            toolData.playerController.entity.onEntityActionStart += StartMoveToAction;
            toolData.playerController.entity.onEntityActionEnd += EndMoveToAction;
            tween = actionIcon.transform.DOBlendableLocalMoveBy(Vector3.up * 50.0f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            tween.Pause();
        }
        Vector3 startPos;
        Tween tween;
        Coroutine routine;
        void StartMoveToAction(Vector3 worldDest)
        {
            if (routine != null)
                StopCoroutine(routine);

            if (!tween.IsPlaying())
                tween.Play();

            moveToActionUIObject.SetActive(true);


            if (toolData.currentTool.icon == null)
            {
                actionIcon.gameObject.SetActive(false);
            }

            else
            {
                actionIcon.sprite = toolData.currentTool.icon;
                actionIcon.gameObject.SetActive(true);
            }


            (moveToActionUIObject.transform as RectTransform).anchoredPosition = Camera.main.WorldToScreenPoint(worldDest);
            routine = StartCoroutine(SetUIPosition(worldDest));
        }

        IEnumerator SetUIPosition(Vector3 worldDest)
        {
            while (moveToActionUIObject.activeSelf)
            {
                (moveToActionUIObject.transform as RectTransform).anchoredPosition = Camera.main.WorldToScreenPoint(worldDest);
                yield return null;
            }
        }

        void EndMoveToAction(Vector3 worldDest) => EndMoveToAction();
        void EndMoveToAction()
        {
            tween.Pause();
            actionIcon.transform.localPosition = startPos;
            moveToActionUIObject.SetActive(false);
        }
    }
}
