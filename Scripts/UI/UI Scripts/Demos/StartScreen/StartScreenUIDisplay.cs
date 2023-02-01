using Core.SceneManagement;
using Sirenix.OdinInspector;

namespace GrandmaGreen
{
    public class StartScreenUIDisplay : UIDisplayBase
    {
        // Settings reference.
        public SettingsUIDisplay m_settingsUIDisplay;
        public SpookuleleAudio.ASoundContainer startSFX;
        public void Start()
        {
            RegisterButtonCallback("startButton", OnStartClicked);
            RegisterButtonCallback("settingsButton", OnSettingsClicked);
            
            // Tells the settings UI to open the start screen back up when the exit button is called.
            m_settingsUIDisplay.RegisterButtonCallback("exitButton", OpenUI);
        }

        private void OnStartClicked()
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
