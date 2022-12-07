using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GrandmaGreen.Collections;
using UnityEngine.U2D.Animation;

namespace GrandmaGreen.Garden
{
    public class ToolEquip : MonoBehaviour
    {
        public PlayerToolData playerTools;
        public SpriteResolver toolSpriteResolver;
        public SpriteRenderer spriteRenderer;
        public CollectionsSO collections;
        static readonly string SPRITE_CATEGORY = "Tools";

        void OnEnable()
        {
            playerTools.onToolSelected += SetEquippedTool;
        }

        void OnDisable()
        {
            playerTools.onToolSelected -= SetEquippedTool;
        }

        void SetEquippedTool(ToolData toolData)
        {
            if (toolData.toolIndex == 3)
            {
                toolSpriteResolver.SetCategoryAndLabel(SPRITE_CATEGORY, "Empty");
                //spriteRenderer.sprite = collections.GetSprite(playerTools.equippedSeed, playerTools.equippedSeedGenotype);
                return;
            }

            toolSpriteResolver.SetCategoryAndLabel(SPRITE_CATEGORY, toolData.toolName);
        }
    }
}
