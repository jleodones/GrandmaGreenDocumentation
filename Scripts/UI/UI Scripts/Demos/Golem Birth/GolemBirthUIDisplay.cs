using System.Collections.Generic;
using GrandmaGreen.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen.UI.Golems
{
    public class GolemBirthUIDisplay : UIDisplayBase
    {
        private VisualElement cloudsA;
        private VisualElement cloudsB;
        private VisualElement leftCloud;
        private VisualElement rightCloud;
        private VisualElement golemContainer;
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
            golemContainer = m_rootVisualElement.Q<VisualElement>("golemSpriteContainer");
            golem = m_rootVisualElement.Q<VisualElement>("golemSprite");
            title = m_rootVisualElement.Q<Label>("title");
            golemName = m_rootVisualElement.Q<Label>("golemName");
            description = m_rootVisualElement.Q<Label>("description");
        }

        public override void Load()
        {
            EventManager.instance.EVENT_GOLEM_SPAWN += OpenUIHandler;
            m_rootVisualElement.RegisterCallback<ClickEvent>(CloseUIHandler);
        }

        public override void Unload()
        {
            EventManager.instance.EVENT_GOLEM_SPAWN -= OpenUIHandler;
            m_rootVisualElement.UnregisterCallback<ClickEvent>(CloseUIHandler);
        }

        [Button()]
        public void TestHandler(ushort golemID)
        {
            OpenUIHandler(golemID, new Vector3());
        }
        
        public void OpenUIHandler(ushort golemID, Vector3 spawnLocation)
        {
            // Switch out the sprite and name.
            string g = ((CharacterId)golemID).ToString();

            Sprite s = Resources.Load<Sprite>("Baby Golems/CHA_" + g + "_Baby");
            //golem.style.backgroundImage = new StyleBackground(s);
            golemName.text = "Baby " + g;
            
            OpenUI();
        }

        public override void UIOpenLogic()
        {
            base.UIOpenLogic();

            //Start Animations
            golemContainer.RegisterCallback<TransitionEndEvent>(BounceGolem);
            golemContainer.style.bottom = Length.Percent(125);

            title.RegisterCallback<TransitionEndEvent>(LargeText);
            m_rootVisualElement.schedule.Execute(() => title.ToggleInClassList("smallText")).StartingIn(2504);
            m_rootVisualElement.schedule.Execute(() => golemName.ToggleInClassList("smallText")).StartingIn(2503);

            m_rootVisualElement.schedule.Execute(() => cloudsB.ToggleInClassList("floatClouds")).StartingIn(501);
            m_rootVisualElement.schedule.Execute(() => cloudsA.ToggleInClassList("floatClouds")).StartingIn(502);

            m_rootVisualElement.schedule.Execute(() => leftCloud.ToggleInClassList("pushLeftCloud")).StartingIn(401);
            m_rootVisualElement.schedule.Execute(() => rightCloud.ToggleInClassList("pushRightCloud")).StartingIn(400);

            description.RegisterCallback<TransitionEndEvent>(ChangeOpacity);
            m_rootVisualElement.schedule.Execute(() =>  description.style.opacity = description.resolvedStyle.opacity + .02f).StartingIn(1300);
        }

        public void CloseUIHandler(ClickEvent ce)
        {
            CloseUI();
            golemContainer.style.bottom = Length.Percent(-50);
            cloudsB.ToggleInClassList("floatClouds");
            cloudsA.ToggleInClassList("floatClouds");
            leftCloud.ToggleInClassList("pushLeftCloud");
            rightCloud.ToggleInClassList("pushRightCloud");
            golemContainer.UnregisterCallback<TransitionEndEvent>(BounceGolem);
            description.UnregisterCallback<TransitionEndEvent>(ChangeOpacity);
            description.style.opacity = description.resolvedStyle.opacity * 0;
            title.style.opacity = description.resolvedStyle.opacity * 0;
            golemName.style.opacity = description.resolvedStyle.opacity * 0;
        }

        private void BounceGolem(TransitionEndEvent evt)
        {
            golemContainer.RegisterCallback<TransitionEndEvent>(BounceGolem);
            golemContainer.ToggleInClassList("bounceGolem");
        }

        private void ChangeOpacity(TransitionEndEvent evt)
        {
            description.style.opacity = description.resolvedStyle.opacity + .02f;
            title.style.opacity = description.resolvedStyle.opacity + 0.2f;
            golemName.style.opacity = description.resolvedStyle.opacity + 0.2f;
            if (description.style.opacity == 1)
            {
                description.UnregisterCallback<TransitionEndEvent>(ChangeOpacity);
            }
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
