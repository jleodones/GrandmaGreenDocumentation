using GrandmaGreen.UI.HUD;
using UnityEngine;
using UnityEngine.UIElements;

namespace GrandmaGreen
{
    public class DemoUIScript : MonoBehaviour
    {
        private UIDocument m_UIDocument;
        void Start()
        {
            m_UIDocument = GetComponent<UIDocument>();
            m_UIDocument.rootVisualElement.Q<Button>("exitButton").RegisterCallback<ClickEvent>(RegisterExitButton);
        }

        public void OpenUI()
        {
            m_UIDocument.rootVisualElement.Q("rootElement").style.display = DisplayStyle.Flex;
        }

        private void RegisterExitButton(ClickEvent cvt)
        {
            m_UIDocument.rootVisualElement.Q("rootElement").style.display = DisplayStyle.None;
        }
    }
}
