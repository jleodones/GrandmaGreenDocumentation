using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GrandmaGreen
{

    public class CameraZoom : MonoBehaviour
    {
        [SerializeField] Cinemachine.CinemachineVirtualCamera zoomCamera;

        Tween zoomTween;
        public void ZoomCameraRequest(float zoom, float time)
        {
            if (zoomTween != null && DOTween.IsTweening(zoomTween))
            {
                zoomTween.Complete();
            }
            float zoomCurrent = zoomCamera.m_Lens.OrthographicSize;

            zoomTween = DOTween.To(() => zoomCurrent, x => zoomCurrent = x, zoom, time)
                        .SetEase(Ease.OutSine)
                        .OnUpdate(() => zoomCamera.m_Lens.OrthographicSize = zoomCurrent);
        }

        public void ZoomCameraRequestNPC(float zoom, float time, GameObject cameraTarget)
        {
            if (zoomTween != null && DOTween.IsTweening(zoomTween))
            {
                zoomTween.Complete();
            }
            float zoomCurrent = zoomCamera.m_Lens.OrthographicSize;
            SetCameraFollow(cameraTarget.transform);
            zoomTween = DOTween.To(() => zoomCurrent, x => zoomCurrent = x, zoom, time)
                        .SetEase(Ease.OutSine)
                        .OnUpdate(() => zoomCamera.m_Lens.OrthographicSize = zoomCurrent);
        }

        public void SetCameraFollow(Transform targetTransform)
        {
            zoomCamera.Follow = targetTransform;
        }

    }
}
