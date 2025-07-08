using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ErisJGDK.Base.Settings
{
    public class ResolutionManager : MonoBehaviour, ISetting
    {
        private int _resolutionIndex = 0;
        private bool _fullscreen = false;

        [SerializeField] private TMP_Dropdown _resolutionDropdown;
        [SerializeField] private Toggle _fullscreenToggle;

        [SerializeField]
        private float[] _allowedAspectRatios = new[]
        {
            16 / 9f,
            854 / 480f,
            1366 / 768f
        };

        public void Load()
        {
            Resolution[] resolutions = GetResolutions();

            _resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", Math.Max(0, resolutions.Length - 1));
            _fullscreen = Convert.ToBoolean(PlayerPrefs.GetInt("Fullscreen", 0));

            _resolutionDropdown.options.Clear();
            foreach (var resolution in resolutions)
            {
                string text = $"{resolution.width}x{resolution.height} @ {Mathf.Floor((float)resolution.refreshRateRatio.value)} Hz";
                _resolutionDropdown.options.Add(new(text));
            }

            _resolutionDropdown.value = _resolutionIndex;
            _fullscreenToggle.isOn = _fullscreen;
        }

        public Resolution[] GetResolutions()
        {
            return Screen.resolutions.Where(r => _allowedAspectRatios.Contains((float)r.width / r.height)).ToArray();
        }

        public void Save()
        {
            _resolutionIndex = _resolutionDropdown.value;
            _fullscreen = _fullscreenToggle.isOn;

            PlayerPrefs.SetInt("ResolutionIndex", _resolutionIndex);
            PlayerPrefs.SetInt("Fullscreen", Convert.ToInt32(_fullscreen));
        }

        public void Apply()
        {
            Resolution[] resolutions = GetResolutions();
            if (resolutions.Length == 0)
                return;

            Resolution resolution = resolutions[Math.Min(_resolutionIndex, resolutions.Length)];

            Screen.SetResolution(resolution.width, resolution.height,
                _fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed, resolution.refreshRateRatio);
        }
    }
}