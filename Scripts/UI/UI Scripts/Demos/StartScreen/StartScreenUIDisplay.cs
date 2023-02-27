using Core.SceneManagement;
using GrandmaGreen.UI.Settings;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;



namespace GrandmaGreen.UI.StartScreen
{
    public class StartScreenUIDisplay : UIDisplayBase
    {
        // Settings reference.
        public SettingsUIDisplay m_settingsUIDisplay;
        public SpookuleleAudio.ASoundContainer startSFX;
        private Button startButton;
        private VisualElement splashScreen;
        public void Start()
        {
            startButton = m_rootVisualElement.Q<Button>("startButton");
            splashScreen = m_rootVisualElement.Q<VisualElement>("splashScreen");
            splashScreen.RegisterCallback<ClickEvent>(OnStartClicked);
            RegisterButtonCallback("settingsButton", OnSettingsClicked);

            // Tells the settings UI to open the start screen back up when the exit button is called.
            // m_settingsUIDisplay.RegisterButtonCallback("exitButton", OpenUI);

            StartAnimation();
        }

        private void StartAnimation() {
            startButton.RegisterCallback<TransitionEndEvent>(evt => startButton.ToggleInClassList("expanded"));
            m_rootVisualElement.schedule.Execute(() => startButton.ToggleInClassList("expanded")).StartingIn(100);
        }

        private void OnStartClicked(ClickEvent evt)
        {
            SceneExtensions.LoadAsync(SCENES.SetupTest);
            startSFX.Play();
        }

        private void OnSettingsClicked()
        {
            CloseUI();
            m_settingsUIDisplay.OpenUI();
        }
    }
}
