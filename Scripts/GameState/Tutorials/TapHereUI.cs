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
            animTween = transform.DOPunchScale(Vector3.one * 0.4f, pulseTime, 10, 0.5f).SetLoops(-1);
        }

        void OnDisable()
        {
            animTween.Kill(true);
            transform.localScale = Vector3.one;
        }
    }
}
