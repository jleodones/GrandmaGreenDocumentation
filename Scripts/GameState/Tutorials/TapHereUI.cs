using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GrandmaGreen
{
    public class TapHereUI : MonoBehaviour
    {
        Tween animTween;
        float pulseTime = 1.0f;

        void OnEnable()
        {
            animTween = transform.DOScale(Vector3.one * 1.35f, pulseTime / 2).SetLoops(-1,LoopType.Yoyo);
        }

        void OnDisable()
        {
            animTween.Kill(true);
            transform.localScale = Vector3.one;
        }
    }
}
