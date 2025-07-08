using System;
using ErisJGDK.Base.Miscellaneous;
using UnityEngine;
using UnityEngine.UI;

namespace ErisJGDK.Base.Settings
{
    [Serializable]
    public struct AudioVolumeSetting
    {
        public Slider Slider;
        public string AudioChannelName;
        public AudioChannel AudioChannel => AudioManager.Instance.GetChannel(AudioChannelName);
    }

    public class AudioVolumeManager : MonoBehaviour, ISetting
    {
        [SerializeField] private AudioVolumeSetting[] _volumeSettings;

        public void Load()
        {
            foreach (var setting in _volumeSettings)
                setting.Slider.value = setting.AudioChannel.Volume;
        }

        public void Save()
        {
            foreach (var setting in _volumeSettings)
                PlayerPrefs.SetFloat("Volume_" + setting.AudioChannelName, setting.Slider.value);
        }

        public void Apply()
        {
            foreach (var setting in _volumeSettings)
            {
                setting.AudioChannel.Volume = setting.Slider.value;
                setting.AudioChannel.SetVolume();
            }
        }
    }
}