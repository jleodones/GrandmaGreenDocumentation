using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrandmaGreen.Garden
{
    [CreateAssetMenu(menuName = "GrandmaGreen/Garden/Tools Data")]
    public class ToolStateData : ScriptableObject
    {
        public Entities.EntityController target;
        public bool toolsOpened = false;
        public event System.Action onToolSelectionStart;
        public event System.Action onToolSelectionEnd;
        public event System.Action<string> onToolSelected;
        public Sprite currentToolicon;
        [SerializeField] Sprite[] toolIcons;
        public string currentTool = "";

        public void ToggleToolSelection()
        {
            toolsOpened = !toolsOpened;
            if (toolsOpened)
                StartToolSelection();
            else
                EndToolSelection();
        }

        public void StartToolSelection()
        {
            if (target.entity.isPathing)
                target.InterruptMovement();

            toolsOpened = true;

            onToolSelectionStart?.Invoke();
        }
        public void EndToolSelection()
        {
            toolsOpened = false;
            onToolSelectionEnd?.Invoke();
        }

        public void SetTool(string tool)
        {
            currentTool = tool;

            switch (currentTool)
            {
                case "trowel":
                    currentToolicon = toolIcons[0];
                    break;
                case "seeds":
                currentToolicon = toolIcons[1];
                    break;
                case "fertilizer":
                currentToolicon = toolIcons[2];
                    break;
                case "watering":
                currentToolicon = toolIcons[3];
                    break;
                default:
                    break;
            }

            onToolSelected?.Invoke(currentTool);
        }

    }
}
