using UnityEngine;
using UnityEngine.UIElements;


namespace GrandmaGreen
{
    public class GolemHappinessMeterController : MonoBehaviour
    {
        //Variables
        private VisualElement m_rootVisualElement;
        private VisualElement meter;
        private VisualElement meterEdge;
        private VisualElement golemMeterIcon;
        private float mockGolemHappiness = 0.0f;
        private float meterEdgeOffset = -1.3f;
        private float difference;
        private Sprite sadIcon;
        private Sprite neutralIcon;
        private Sprite happyIcon;

        public GolemHappinessMeterController(VisualElement m_rootVisualElement, Sprite sad, Sprite neutral, Sprite happy)
        {
            this.m_rootVisualElement = m_rootVisualElement;
            meter = m_rootVisualElement.Q(className: "unity-progress-bar__progress");
            meterEdge = m_rootVisualElement.Q("meterEdge");
            golemMeterIcon = m_rootVisualElement.Q("golemMeterIcon");
            sadIcon = sad;
            neutralIcon = neutral;
            happyIcon = happy;

            golemMeterIcon.RegisterCallback<ClickEvent>(OnMeterClick);
        }

        //Updated the Golem Meter based on new total percentage (float)
        public void UpdateGolemMeter(float percent)
        {
            //Set correct emotion icon
            golemMeterIcon.style.backgroundImage = new StyleBackground(sadIcon);
            if (percent > 66f)
            {
                golemMeterIcon.style.backgroundImage = new StyleBackground(happyIcon);
            }
            else if (percent > 33f)
            {
                golemMeterIcon.style.backgroundImage = new StyleBackground(neutralIcon);
            }

            //Update progress bar fill
            difference = 100f - percent;
            if (difference > 100f)
            {
                difference = 100f;
            }
            else if (difference < 0)
            {
                difference = 0f;
                CloseGolemMeter();
            }
            meter.style.width = Length.Percent(difference);
            meterEdge.style.right = Length.Percent(difference + meterEdgeOffset);
        }

        private void OnMeterClick(ClickEvent evt)
        {
            mockGolemHappiness += 10f;
            UpdateGolemMeter(mockGolemHappiness);
        }
        public void CloseGolemMeter()
        {
            m_rootVisualElement.Q("happinessMeter").style.display = DisplayStyle.None;
        }
        public void OpenGolemMeter()
        {
            m_rootVisualElement.Q("happinessMeter").style.display = DisplayStyle.Flex;
        }
    }
}
