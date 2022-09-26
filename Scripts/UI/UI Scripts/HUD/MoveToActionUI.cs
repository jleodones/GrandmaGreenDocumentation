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

        public ToolStateData toolState;
        public EntityController target;
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
            target.entity.onEntityActionStart += StartMoveToAction;
            target.entity.onEntityActionEnd += EndMoveToAction;

        }
        Vector3 startPos;
        Tween tween;
        Coroutine routine;
        void StartMoveToAction(Vector3 worldDest)
        {
            if (routine != null)
                StopCoroutine(routine);


            if (!DOTween.IsTweening(tween))
            {
                tween = actionIcon.transform.DOBlendableLocalMoveBy(Vector3.up * 50.0f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            }


            moveToActionUIObject.SetActive(true);
            actionIcon.sprite = toolState.currentToolicon;

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

            tween.Kill();
            actionIcon.transform.localPosition = startPos;
            moveToActionUIObject.SetActive(false);
        }
    }
}
