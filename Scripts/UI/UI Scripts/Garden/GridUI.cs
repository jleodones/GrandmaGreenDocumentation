using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GrandmaGreen.Garden
{
    public class GridUI : MonoBehaviour
    {
        public PlayerToolData toolData;
        public Material gridMaterial;
        public float fadeTime = 0.5f;
        public float maxOpacity = 0.1f;
        bool gridActive = false;
        Tween fader;
        void OnEnable()
        {
            toolData.onToolSelected += CheckTool;
            CheckTool(toolData.currentTool);
        }


        void OnDisable()
        {
            toolData.onToolSelected -= CheckTool;
            DisableGrid(0);
        }

        void CheckTool(ToolData tool)
        {
            if (tool.toolIndex == 0 && gridActive)
            {
                DisableGrid(fadeTime);
            }
            else if (tool.toolIndex != 0 && !gridActive)
            {
                EnableGrid(fadeTime);
            }
        }


        void EnableGrid(float time)
        {
            gridActive = true;
            FadeGrid(maxOpacity, time);
        }

        void DisableGrid(float time)
        {
            gridActive = false;
            FadeGrid(0, time);
        }

        void FadeGrid(float fade, float time)
        {
            if (fader.IsActive())
                fader.Complete();

            fader = gridMaterial.DOFade(fade, time);
        }
    }
}
