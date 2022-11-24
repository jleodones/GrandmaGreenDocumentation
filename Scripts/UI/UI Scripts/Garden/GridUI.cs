using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GrandmaGreen.Garden
{
    public class GridUI : MonoBehaviour
    {
        public PlayerToolData toolData;
        public GardenCustomizer customizer;
        public Material gridMaterial;
        public float fadeTime = 0.5f;
        public float maxOpacity = 0.1f;
        bool gridActive = false;
        Tween fader;
        void OnEnable()
        {
            DisableGrid(0);
            toolData.onToolSelected += CheckTool;
            customizer.onCustomizationStart += EnableGrid;
            customizer.onCustomizationEnd += DisableGrid;
            CheckTool(toolData.currentTool);


        }


        void OnDisable()
        {
            toolData.onToolSelected -= CheckTool;
            customizer.onCustomizationStart -= EnableGrid;
            customizer.onCustomizationEnd -= DisableGrid;
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

        public void EnableGrid()
        {
            if (!gridActive)
                EnableGrid(fadeTime);
        }

        public void DisableGrid()
        {
            if (toolData.currentTool.toolIndex == 0 && gridActive)
                DisableGrid(fadeTime);
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
