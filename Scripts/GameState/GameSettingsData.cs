using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;


namespace GrandmaGreen
{

    [System.Serializable]
    public struct SettingsSet
    {
        public float masterVolume;
        public float sfxVolume;
        public float musicVolume;
        public bool batterySaver;
    }

    [CreateAssetMenu(fileName = "GameSettingsData", menuName = "GrandmaGreen/SettingsData", order = 0)]
    public class GameSettingsData : ScriptableObject
    {

        public static readonly string masterVolumeParameter = "VOLUME_MASTER";
        public static readonly string sfxVolumeParameter = "VOLUME_SFX";
        public static readonly string musicVolumeParameter = "VOLUME_MUSIC";
        public static readonly string batterySaverParameter = "BATTERY_SAVER";

        [SerializeField] AudioMixer masterMixer;
        [SerializeField] RenderPipelineAsset renderAsset_Default;
        [SerializeField] RenderPipelineAsset renderAsset_BatterySaver;

        private SettingsSet m_currentSettings;

        public SettingsSet Current => m_currentSettings;
        
        public bool muted { get; private set; }

        public void LoadSettings()
        {
            SetMasterVolume(PlayerPrefs.GetFloat(masterVolumeParameter, 0));
            SetMusicVolume(PlayerPrefs.GetFloat(musicVolumeParameter, 0));
            SetSFXVolume(PlayerPrefs.GetFloat(sfxVolumeParameter, 0));

            SetBatterySaverMode(PlayerPrefs.GetInt(batterySaverParameter, 0) == 0 ? false : true);
        }


        public void SaveSettings()
        {
            PlayerPrefs.SetFloat(masterVolumeParameter, m_currentSettings.masterVolume);
            PlayerPrefs.SetFloat(musicVolumeParameter, m_currentSettings.musicVolume);
            PlayerPrefs.SetFloat(sfxVolumeParameter, m_currentSettings.sfxVolume);

            PlayerPrefs.SetInt(batterySaverParameter, m_currentSettings.batterySaver ? 1 : 0);
        }


        public void SetMasterVolume(float value)
        {
            m_currentSettings.masterVolume = value;
            masterMixer.SetFloat(masterVolumeParameter, value);
            SaveSettings();
        }

        public void SetMusicVolume(float value)
        {
            m_currentSettings.musicVolume = value;
            masterMixer.SetFloat(musicVolumeParameter, value);
            SaveSettings();
        }

        public void SetSFXVolume(float value)
        {
            m_currentSettings.sfxVolume = value;
            masterMixer.SetFloat(sfxVolumeParameter, value);
            SaveSettings();
        }

        [ContextMenu("Mute")]
        public void MuteMasterVolume()
        {
            masterMixer.SetFloat(masterVolumeParameter, -80);
            muted = true;
        }

        [ContextMenu("Unmute")]
        public void UnmuteMasterVolume()
        {
            masterMixer.SetFloat(masterVolumeParameter, m_currentSettings.masterVolume);
            muted = false;
        }

        public void SetBatterySaverMode(bool value)
        {
            if (!value)
            {
                DisableBatterySaverMode();
            }
            else
            {
                EnableBatterySaverMode();

            }
        }

        [ContextMenu("EnableBatterySaverMode")]
        public void EnableBatterySaverMode()
        {
            m_currentSettings.batterySaver = true;

            GraphicsSettings.defaultRenderPipeline = renderAsset_BatterySaver;
            QualitySettings.renderPipeline = renderAsset_BatterySaver;
            Application.targetFrameRate = 30;
        }

        [ContextMenu("DisableBatterySaverMode")]
        public void DisableBatterySaverMode()
        {
            m_currentSettings.batterySaver = false;

            GraphicsSettings.defaultRenderPipeline = renderAsset_Default;
            QualitySettings.renderPipeline = renderAsset_Default;
            Application.targetFrameRate = -1;
        }
    }
}
