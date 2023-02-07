using Core.SceneManagement;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;



namespace GrandmaGreen
{
    public class StartScreenUIDisplay : UIDisplayBase
    {
        // Settings reference.
        public SettingsUIDisplay m_settingsUIDisplay;
        public SpookuleleAudio.ASoundContainer startSFX;
        public bool onStartScreen = true;
        private Button startButton;
        public void Start()
        {
            startButton = m_rootVisualElement.Q<Button>("startButton");
            RegisterButtonCallback("startButton", OnStartClicked);
            RegisterButtonCallback("settingsButton", OnSettingsClicked);

            // Tells the settings UI to open the start screen back up when the exit button is called.
            m_settingsUIDisplay.RegisterButtonCallback("exitButton", OpenUI);

            StartAnimation();
        }

        private void StartAnimation() {
            startButton.RegisterCallback<TransitionEndEvent>(evt => startButton.ToggleInClassList("expanded"));
            m_rootVisualElement.schedule.Execute(() => startButton.ToggleInClassList("expanded")).StartingIn(100);
        }

        private void OnStartClicked()
        {
            onStartScreen = false;
            SceneExtensions.LoadAsync(SCENES.SetupTest);
            startSFX.Play();
        }

        private void OnSettingsClicked()
        {
            onStartScreen = false;
            CloseUI();
            m_settingsUIDisplay.OpenUI();
        }
    }
}
