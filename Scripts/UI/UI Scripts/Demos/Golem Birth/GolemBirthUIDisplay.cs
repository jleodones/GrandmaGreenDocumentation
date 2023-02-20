using System;
using System.Collections.Generic;
using GrandmaGreen.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen
{
    public class GolemBirthUIDisplay : UIDisplayBase
    {
        private VisualElement cloudsA;
        private VisualElement cloudsB;
        private VisualElement leftCloud;
        private VisualElement rightCloud;
        private VisualElement golem;
        private Label title;
        private Label golemName;
        private Label description;

        public void Start()
        {
            cloudsA = m_rootVisualElement.Q<VisualElement>("cloudsBG1");
            cloudsB = m_rootVisualElement.Q<VisualElement>("cloudsBG2");
            leftCloud = m_rootVisualElement.Q<VisualElement>("leftCloud");
            rightCloud = m_rootVisualElement.Q<VisualElement>("rightCloud");
            golem = m_rootVisualElement.Q<VisualElement>("golemSprite");
            title = m_rootVisualElement.Q<Label>("title");
            golemName = m_rootVisualElement.Q<Label>("golemName");
            description = m_rootVisualElement.Q<Label>("description");
            OpenUI();
        }

        public override void OpenUI()
        {
            base.OpenUI();
            //Start Animations
            m_rootVisualElement.schedule.Execute(() => cloudsA.ToggleInClassList("floatClouds")).StartingIn(100);
            m_rootVisualElement.schedule.Execute(() => cloudsB.ToggleInClassList("floatClouds")).StartingIn(100);
            m_rootVisualElement.schedule.Execute(() => leftCloud.ToggleInClassList("pushLeftCloud")).StartingIn(500);
            m_rootVisualElement.schedule.Execute(() => rightCloud.ToggleInClassList("pushRightCloud")).StartingIn(500);
            golem.RegisterCallback<TransitionEndEvent>(BounceGolem);
            m_rootVisualElement.schedule.Execute(() => golem.ToggleInClassList("pushGolem")).StartingIn(100);
            title.RegisterCallback<TransitionEndEvent>(LargeText);
            m_rootVisualElement.schedule.Execute(() => title.ToggleInClassList("smallText")).StartingIn(100);
            m_rootVisualElement.schedule.Execute(() => golemName.ToggleInClassList("smallText")).StartingIn(100);
            description.style.transitionProperty = new List<StylePropertyName> { "opacity" };
            description.style.transitionDuration = new List<TimeValue> { new TimeValue(200, TimeUnit.Millisecond) };
            description.style.opacity = description.resolvedStyle.opacity + 100;
        }

        private void BounceGolem(TransitionEndEvent evt)
        {
            /*if (golem.ClassListContains("pushGolem"))
            {
                golem.style.translate = new Translate(0, Length.Percent(-110), 0);
                golem.RemoveFromClassList("pushGolem");
            }
            golem.RegisterCallback<TransitionEndEvent>(BounceGolem);*/
            golem.ToggleInClassList("bounceGolem");
        }

        private void LargeText(TransitionEndEvent evt)
        {
            title.UnregisterCallback<TransitionEndEvent>(LargeText);
            title.ToggleInClassList("smallText");
            golemName.ToggleInClassList("smallText");
            title.RegisterCallback<TransitionEndEvent>(NormalText);
            title.ToggleInClassList("largeText");
            golemName.ToggleInClassList("largeText");

        }

        private void NormalText(TransitionEndEvent evt)
        {
            title.UnregisterCallback<TransitionEndEvent>(NormalText);
            title.ToggleInClassList("largeText");
            golemName.ToggleInClassList("largeText");

        }

        void SetText(string id, string newText)
        {
            m_rootVisualElement.Q<Label>(id).text = newText;
        }

        void SetSprite(string elementName, Sprite sprite) {
            m_rootVisualElement.Q(elementName).style.backgroundImage = new StyleBackground(sprite);

        }

    }
}
