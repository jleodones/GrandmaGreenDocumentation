using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SetCameraBounds : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;
    public Transform cameraVolume;

    [ContextMenu("Calc Bounds")]
    public void CalculateBounds()
    {
        Vector3 volumeSize = cameraVolume.localScale;
        Vector3 volumeCenter = cameraVolume.position;

        volumeCenter.x = vCam.transform.position.x;
        volumeCenter.y = vCam.transform.position.z;
        volumeCenter.z = 0;

        volumeSize.y = (int)(volumeCenter.y * 2);

        cameraVolume.localScale = volumeSize;
        cameraVolume.position = volumeCenter;

    }

}
