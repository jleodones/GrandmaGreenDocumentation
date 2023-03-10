using System.Collections.Generic;
using GrandmaGreen.Collections;
using GrandmaGreen.Entities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen.UI.Golems
{
    public class GolemEvolutionUIDisplay : UIDisplayBase
    {
        private VisualElement shootingStarTrail;
        private VisualElement shootingStarsBG;
        private VisualElement shootingStarsFG;
        private VisualElement golem;
        private Label title;
        private Label golemName;
        private float opacityCount = 0.2f;
        
        public GolemManager golemManager;

        public void Start()
        {
            shootingStarTrail = m_rootVisualElement.Q<VisualElement>("shootingStarTrailContainer");
            shootingStarsBG = m_rootVisualElement.Q<VisualElement>("shootingStarsBGContainer");
            shootingStarsFG = m_rootVisualElement.Q<VisualElement>("shootingStarsFGContainer");
            golem = m_rootVisualElement.Q<VisualElement>("golemSprite");
            title = m_rootVisualElement.Q<Label>("title");
            golemName = m_rootVisualElement.Q<Label>("golemName");
        }

        public override void Load()
        {
            EventManager.instance.EVENT_GOLEM_EVOLVE += OpenUIHandler;
            m_rootVisualElement.RegisterCallback<ClickEvent>(CloseUIHandler);
        }

        public override void Unload()
        {
            EventManager.instance.EVENT_GOLEM_EVOLVE -= OpenUIHandler;
            m_rootVisualElement.UnregisterCallback<ClickEvent>(CloseUIHandler);
        }

        [Button()]
        public void TestHandler(ushort golemID)
        {
            OpenUIHandler(golemID);
        }
        
        public void OpenUIHandler(ushort golemID)
        {
            // Switch out the sprite and name.
            string g = ((CharacterId)golemID).ToString();

            Sprite s = Resources.Load<Sprite>("Baby Golems/CHA_" + g + "_Baby");
            golemName.text = "Baby " + g;
            golem.style.backgroundImage = new StyleBackground(s);
            
            OpenUI();
        }

        public override void UIOpenLogic()
        {
            base.UIOpenLogic();

            //Start Animations
            m_rootVisualElement.schedule.Execute(() => shootingStarTrail.ToggleInClassList("pushIn")).StartingIn(104);
            m_rootVisualElement.schedule.Execute(() => shootingStarsBG.ToggleInClassList("pushIn")).StartingIn(103);
            m_rootVisualElement.schedule.Execute(() => shootingStarsFG.ToggleInClassList("pushIn")).StartingIn(102);
            m_rootVisualElement.schedule.Execute(() => golem.ToggleInClassList("pushIn")).StartingIn(101);

            title.RegisterCallback<TransitionEndEvent>(ChangeOpacity);
            m_rootVisualElement.schedule.Execute(() => title.style.opacity = golemName.resolvedStyle.opacity + .2f).StartingIn(1301);
            m_rootVisualElement.schedule.Execute(() => title.style.opacity = title.resolvedStyle.opacity + .2f).StartingIn(1300);
        }

        public void CloseUIHandler(ClickEvent ce)
        {
            CloseUI();
            shootingStarTrail.ToggleInClassList("pushIn");
            shootingStarsBG.ToggleInClassList("pushIn");
            shootingStarsFG.ToggleInClassList("pushIn");
            golem.ToggleInClassList("pushIn");
            //title.UnregisterCallback<TransitionEndEvent>(ChangeOpacity);
            title.style.opacity = title.resolvedStyle.opacity * 0;
            golemName.style.opacity = golemName.resolvedStyle.opacity * 0;
            opacityCount = 0.2f;
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

        private void ChangeOpacity(TransitionEndEvent evt)
        {
            title.style.opacity = title.resolvedStyle.opacity + 0.2f;
            golemName.style.opacity = golemName.resolvedStyle.opacity + 0.2f;
            opacityCount += 0.2f;
            if (opacityCount >= 1.0f)
            {
                title.UnregisterCallback<TransitionEndEvent>(ChangeOpacity);
                title.RegisterCallback<TransitionEndEvent>(LargeText);
                m_rootVisualElement.schedule.Execute(() => title.ToggleInClassList("smallText")).StartingIn(2);
                m_rootVisualElement.schedule.Execute(() => golemName.ToggleInClassList("smallText")).StartingIn(1);
            }
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
