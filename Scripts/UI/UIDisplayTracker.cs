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
        public Dictionary<UITag, UIDisplayRule> displayRules = new Dictionary<UITag, UIDisplayRule>();
        
        // static List<UIDisplayBase> activeUIDisplays;
        static Dictionary<UITag, List<UIDisplayBase>> trackedUIDisplays;
        static Dictionary<UITag, UIDisplayRule> DisplayRules;

        public void Initalize()
        {
            DisplayRules = displayRules;
            trackedUIDisplays = new Dictionary<UITag, List<UIDisplayBase>>();
            // activeUIDisplays = new List<UIDisplayBase>();
            
            Debug.Log(displayRules);
            Debug.Log(DisplayRules);
        }

        public void Release()
        {
            DisplayRules = null;
            trackedUIDisplays = null;
        }

        public static void AddPanel(UIDisplayBase uiDisplay)
        {
            if (uiDisplay.panelTags == 0 || uiDisplay.panelTags == UITag.None)
                return;
            
            trackedUIDisplays.TryAdd(uiDisplay.panelTags, new List<UIDisplayBase>());
            trackedUIDisplays.TryAdd(UITag.ALL, new List<UIDisplayBase>());
            
            trackedUIDisplays[uiDisplay.panelTags].Add(uiDisplay);
            trackedUIDisplays[UITag.ALL].Add(uiDisplay);
        }

        public static void RemovePanel(UIDisplayBase uiDisplay)
        {
            if (trackedUIDisplays != null && uiDisplay.panelTags != UITag.None)
            {
                trackedUIDisplays[uiDisplay.panelTags].Remove(uiDisplay);
                trackedUIDisplays[UITag.ALL].Remove(uiDisplay);
            }
        }

        public static void ProcessOpenDisplayRule(UITag tag)
        {
            if (!DisplayRules.TryGetValue(tag, out UIDisplayRule rule))
                return;
            
            // Parse again.
            var list = rule.closeWhenKeyOpened.ToString().Split(", ");
            List<UITag> tags = new List<UITag>();
            foreach (string s in list)
            {
                UITag.TryParse(s, false, out UITag result);
                if (result == UITag.None)
                    continue;
                tags.Add(result);
            }

            foreach (UITag t in tags)
            {
                if (!trackedUIDisplays.TryGetValue(t, out var l))
                    continue;
                foreach (UIDisplayBase display in l)
                {
                    display.UICloseLogic();
                    // activeUIDisplays.Remove(display);
                }
            }

            // Parse again.
            list = rule.openWhenKeyOpened.ToString().Split(", ");
            tags.Clear();
            foreach (string s in list)
            {
                UITag.TryParse(s, false, out UITag result);
                if (result == UITag.None)
                    continue;
                tags.Add(result);
            }

            foreach (UITag t in tags)
            {
                if (!trackedUIDisplays.TryGetValue(t, out var l))
                    continue;
                foreach (UIDisplayBase display in l)
                {
                    display.UIOpenLogic();
                    // activeUIDisplays.Add(display);
                }
            }
        }

        public static void ProcessCloseDisplayRule(UITag tag)
        {
            if (!DisplayRules.TryGetValue(tag, out UIDisplayRule rule))
                return;

            var list = rule.closeWhenKeyClosed.ToString().Split(", ");
            List<UITag> tags = new List<UITag>();
            foreach (string s in list)
            {
                UITag.TryParse(s, false, out UITag result);
                if (result == UITag.None)
                    continue;
                tags.Add(result);
            }

            foreach (UITag t in tags)
            {
                if (!trackedUIDisplays.TryGetValue(t, out var l))
                    continue;
                foreach (UIDisplayBase display in l)
                {
                    display.UICloseLogic();
                    // activeUIDisplays.Remove(display);
                }
            }

            list = rule.openWhenKeyClosed.ToString().Split(", ");
            tags.Clear();
            foreach (string s in list)
            {
                UITag.TryParse(s, false, out UITag result);

                if (result == UITag.None)
                    continue;
                tags.Add(result);
            }

            foreach (UITag t in tags)
            {
                if (!trackedUIDisplays.TryGetValue(t, out var l))
                    continue;
                foreach (UIDisplayBase display in l)
                {
                    display.UIOpenLogic();
                    // activeUIDisplays.Add(display);
                }
            }
        }
    }
}
