using Core.SceneManagement;
using UnityEngine.UIElements;

namespace GrandmaGreen
{
    public class SettingsUIDisplay : UIDisplayBase
    {
        public GameSettingsData gameSettingsData;

        // Holds onto the sliders.
        private Slider m_bgmSlider;
        private Slider m_sfxSlider;

        public SpookuleleAudio.ASoundContainer openSFX;
        public SpookuleleAudio.ASoundContainer closeSFX;
        public SpookuleleAudio.ASoundContainer selectSFX;
        public SpookuleleAudio.ASoundContainer scrollSFX;

        public void Start()
        {
            // Load player preferences.
            gameSettingsData.LoadSettings();

            // BGM sound setup.
            m_bgmSlider = m_rootVisualElement.Q<Slider>("bgmSlider");
            m_bgmSlider.highValue = 20.0f;
            m_bgmSlider.lowValue = -80.0f;

            // Sets the current position based on current player prefs.
            m_bgmSlider.value = gameSettingsData.Current.musicVolume;

            m_bgmSlider.RegisterValueChangedCallback(v =>
            {
                AdjustBGMVolume(v.newValue);
            });

            // SFX sound setup.
            m_sfxSlider = m_rootVisualElement.Q<Slider>("sfxSlider");
            m_sfxSlider.highValue = 20.0f;
            m_sfxSlider.lowValue = -80.0f;

            // Sets the current position based on current player prefs.
            m_sfxSlider.value = gameSettingsData.Current.sfxVolume;

            m_sfxSlider.RegisterValueChangedCallback(v =>
            {
                AdjustSFXVolume(v.newValue);
            });

            // Registering save file delete callback.
            RegisterButtonCallback("deleteButton", DeleteSaveFile);
        }

        public override void OpenUI()
        {
            openSFX.Play();

            base.OpenUI();
        }

        public override void CloseUI()
        {
            closeSFX.Play();

            base.CloseUI();
        }
        SpookuleleAudio.SoundPlayer soundPlayer;
        private void AdjustBGMVolume(float newValue)
        {
            gameSettingsData.SetMusicVolume(newValue);

            if (soundPlayer != null)
                if (soundPlayer.IsPlaying)
                    return;
            soundPlayer = scrollSFX.Play();
        }

        private void AdjustSFXVolume(float newValue)
        {
            gameSettingsData.SetSFXVolume(newValue);

            if (soundPlayer != null)
                if (soundPlayer.IsPlaying)
                    return;

            soundPlayer = scrollSFX.Play();
        }

        private void DeleteSaveFile()
        {
            // Delete save file.
            EventManager.instance.HandleEVENT_DELETE_SAVE();

            // Then reload start scene.
            SceneExtensions.LoadAsync(SCENES.StartScene);

            selectSFX.Play();
        }
    }
}
