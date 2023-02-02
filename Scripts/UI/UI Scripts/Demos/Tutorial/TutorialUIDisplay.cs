using System;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.UIElements;

namespace GrandmaGreen
{
    public class TutorialUIDisplay : UIDisplayBase
    {
        private Button leftArrow;
        private Button rightArrow;
        private Button exitButton;
        private VisualElement pageNavigator;
        private SlideshowData data;
        private int currentPageIndex = 0;

        public override void OpenUI()
        {

            EventManager.instance.HandleEVENT_CLOSE_HUD();
            base.OpenUI();
        }

        public override void CloseUI()
        {

            EventManager.instance.HandleEVENT_OPEN_HUD();
            base.CloseUI();
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

        public void SetUpSlideshow(SlideshowData data)
        {
            OpenUI();
            this.data = data;

            //Disable exit button
            leftArrow.SetEnabled(false);
            if (data.slides.Count > 1)
            {
                exitButton.SetEnabled(false);
                rightArrow.SetEnabled(true);
            }
            else
            {
                exitButton.SetEnabled(true);
                rightArrow.SetEnabled(false);
            }

            currentPageIndex = 0;
            SyncPageNavigator(currentPageIndex);

            SetText("title", data.title);
            FillInData(0);
            //Add page indicators

            pageNavigator.Clear();

            for (int i = 0; i < data.slides.Count; i++)
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
            SlideData page = data.slides[index];
            SetSprite("visual", page.image);
            SetText("caption", page.caption);
        }

        void MockChangeSlide(ClickEvent evt)
        {
            Button arrow = evt.currentTarget as Button;
            if (arrow.name == "leftArrow")
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

                if (currentPageIndex >= data.slides.Count - 1)
                {
                    currentPageIndex = data.slides.Count - 1;
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
