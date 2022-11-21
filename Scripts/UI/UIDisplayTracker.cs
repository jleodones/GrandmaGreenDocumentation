using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GrandmaGreen.UI
{
    [System.Serializable]
    public struct UIDisplayRule
    {
        public UITag closeWhenKeyOpened;
        public UITag openWhenKeyOpened;

        public UITag closeWhenKeyClosed;
        public UITag openWhenKeyClosed;
    }

    [CreateAssetMenu(menuName = "GrandmaGreen/UI/UIDisplayRules")]
    public class UIDisplayTracker : SerializedScriptableObject
    {
        [SerializeField] public Dictionary<UITag, UIDisplayRule> displayRules = new Dictionary<UITag, UIDisplayRule>();

        static Dictionary<UITag, List<BaseUIDisplay>> trackedUIDisplays;
        static Dictionary<UITag, UIDisplayRule> DisplayRules;

        public void Initalize()
        {
            DisplayRules = displayRules;
            trackedUIDisplays = new Dictionary<UITag, List<BaseUIDisplay>>();

            Debug.Log(displayRules);
            Debug.Log(DisplayRules);
        }

        public void Release()
        {
            DisplayRules = null;
            trackedUIDisplays = null;
        }

        public static void AddPanel(BaseUIDisplay uiDisplay)
        {
            if (uiDisplay.PanelTags == 0)
                return;

            trackedUIDisplays.TryAdd(uiDisplay.PanelTags, new List<BaseUIDisplay>());
            trackedUIDisplays[uiDisplay.PanelTags].Add(uiDisplay);

            ProcessOpenDisplayRule(uiDisplay.PanelTags);
        }

        public static void RemovePanel(BaseUIDisplay uiDisplay)
        {
            trackedUIDisplays[uiDisplay.PanelTags].Remove(uiDisplay);

            ProcessCloseDisplayRule(uiDisplay.PanelTags);
        }

        static void ProcessOpenDisplayRule(UITag tag)
        {
            if (!DisplayRules.TryGetValue(tag, out UIDisplayRule rule))
                return;

            foreach (BaseUIDisplay display in trackedUIDisplays[rule.openWhenKeyOpened])
            {
                display.OpenPanel();
            }

            foreach (BaseUIDisplay display in trackedUIDisplays[rule.closeWhenKeyOpened])
            {
                display.ClosePanel();
            }
        }

        static void ProcessCloseDisplayRule(UITag tag)
        {
            if (!DisplayRules.TryGetValue(tag, out UIDisplayRule rule))
                return;

            foreach (BaseUIDisplay display in trackedUIDisplays[rule.openWhenKeyClosed])
            {
                display.OpenPanel();
            }

            foreach (BaseUIDisplay display in trackedUIDisplays[rule.closeWhenKeyClosed])
            {
                display.ClosePanel();
            }
        }
    }
}
