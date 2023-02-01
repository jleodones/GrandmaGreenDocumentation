using GrandmaGreen.Mail;
using System;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.UIElements;

namespace GrandmaGreen
{
    public class MailboxUIDisplay : UIDisplayBase
    {
        private Button leftArrow;
        private Button rightArrow;
        private Button exitButton;
        private VisualElement pageNavigator;
        private List<Letter> data;
        [SerializeField] private MailboxModel mailbox;
        private int currentPageIndex = 0;

        public override void OpenUI()
        {
            data = mailbox.GetLetters();
            base.OpenUI();
            if(data.Count < 1){
                leftArrow.SetEnabled(false);
                rightArrow.SetEnabled(false);
                SetText("body", "You have no mail :(");
            }
            else{
                SetUpMailbox();
            }
        }
        public override void Load()
        {
            //Assign Variables
            leftArrow = m_rootVisualElement.Query<Button>("leftArrow");
            rightArrow = m_rootVisualElement.Query<Button>("rightArrow");
            exitButton = m_rootVisualElement.Query<Button>("exitButton");
            pageNavigator = m_rootVisualElement.Q<VisualElement>("pageNavigator");

            //Register onClick for arrows
            leftArrow.RegisterCallback<ClickEvent>(MockChangeSlide);
            rightArrow.RegisterCallback<ClickEvent>(MockChangeSlide);
      
        }

        public void SetUpMailbox()
        {
            
            //Disable left arrow
            leftArrow.SetEnabled(false);   
            if(data.Count > 1) { 
                rightArrow.SetEnabled(true);
            }
            else
            {
                rightArrow.SetEnabled(false);
            }

            currentPageIndex = 0;
            SyncPageNavigator(currentPageIndex);
            FillInData(0);
            //Add page indicators
            pageNavigator.Clear();
            for(int i = 0; i < data.Count; i++)
            {
                if (i == 0)
                {
                    AddPageToNav("currentCircle");
                }
                else
                {
                    AddPageToNav("circle");
                }
            }
        }

        void FillInData(int index)
        {
            Letter letter = data[index];
            SetText("heading", letter.Heading);
            SetText("body", letter.Body);
            SetText("signature", letter.Signature);
        }

        void MockChangeSlide(ClickEvent evt) {
            Button arrow = evt.currentTarget as Button;
            if(arrow.name == "leftArrow")
            {
                currentPageIndex--;
                rightArrow.SetEnabled(true);
                if (currentPageIndex <= 0)
                {
                    currentPageIndex = 0;
                    leftArrow.SetEnabled(false);
                }
            }
            else
            {
                leftArrow.SetEnabled(true);
                currentPageIndex++;

                if (currentPageIndex >= data.Count - 1)
                {
                    currentPageIndex = data.Count - 1;
                    rightArrow.SetEnabled(false);
                    exitButton.SetEnabled(true);
                }
            }
            FillInData(currentPageIndex);
            SyncPageNavigator(currentPageIndex);
        }

        void SyncPageNavigator(int current)
        {
            IEnumerable<VisualElement> children = pageNavigator.Children();
            int it = 0;
            foreach (VisualElement c in children)
            {
                if (c.ClassListContains("currentCircle"))
                {
                    c.RemoveFromClassList("currentCircle");
                    c.AddToClassList("circle");
                }
            }
            foreach (VisualElement c in children)
            {
                if (it == current)
                {
                    c.RemoveFromClassList("circle");
                    c.AddToClassList("currentCircle");
                }
                it++;
            }
        }

        void AddPageToNav(string className)
        {
            var child = new VisualElement();
            child.AddToClassList(className);
            pageNavigator.Add(child);
        }
        void SetText(string id, string newText)
        {
            m_rootVisualElement.Q<Label>(id).text = newText;
        }
        void SetSprite(string name, Sprite sprite)
        {
            m_rootVisualElement.Q(name).style.backgroundImage = new StyleBackground(sprite);

        }
    }
}
