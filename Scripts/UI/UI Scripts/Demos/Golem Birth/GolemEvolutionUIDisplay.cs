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
        private VisualElement shootingStar1;
        private VisualElement shootingStar2;
        private VisualElement shootingStar3;
        private VisualElement golem;
        private Label title;
        private Label golemName;
        
        public GolemManager golemManager;

        public void Start()
        {
            shootingStar1 = m_rootVisualElement.Q<VisualElement>("shootingStar1");
            shootingStar2 = m_rootVisualElement.Q<VisualElement>("shootingStar2");
            shootingStar3 = m_rootVisualElement.Q<VisualElement>("shootingStar3");
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
            if(s != null) {
                golem.style.backgroundImage = new StyleBackground(s);
            }
            
            OpenUI();
        }

        public override void UIOpenLogic()
        {
            base.UIOpenLogic();

            //Start Animations
            m_rootVisualElement.schedule.Execute(() => BeginAnimation()).StartingIn(145);
        }

        private void BeginAnimation()
        {
            shootingStar1.RemoveFromClassList("fadeIn");
            shootingStar1.RemoveFromClassList("startStar1");

            shootingStar2.RemoveFromClassList("fadeIn");
            shootingStar2.RemoveFromClassList("startStar2");

            shootingStar3.RemoveFromClassList("fadeIn");
            shootingStar3.RemoveFromClassList("startStar3");

            title.RemoveFromClassList("fadeIn");
            golemName.RemoveFromClassList("fadeIn");
        }
        public void CloseUIHandler(ClickEvent ce)
        {
            CloseUI();
            shootingStar1.AddToClassList("fadeIn");
            shootingStar1.AddToClassList("startStar1");

            shootingStar2.AddToClassList("fadeIn");
            shootingStar2.AddToClassList("startStar2");

            shootingStar3.AddToClassList("fadeIn");
            shootingStar3.AddToClassList("startStar3");

            title.AddToClassList("fadeIn");
            golemName.AddToClassList("fadeIn");
        }
    }
}
