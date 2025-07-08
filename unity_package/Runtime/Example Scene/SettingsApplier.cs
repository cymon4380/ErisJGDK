using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ErisJGDK.Base;

namespace ErisJGDK.Example
{
    public class SettingsApplier : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _passwordField;
        [SerializeField] private TMP_InputField _moderationPasswordField;
        [SerializeField] private Toggle _audienceEnabledToggle;

        public void Apply()
        {
            RoomSettings.Password = _passwordField.text;
            RoomSettings.ModerationPassword = _moderationPasswordField.text;
            RoomSettings.AudienceEnabled = _audienceEnabledToggle.isOn;

            RoomSettings.Save();
        }
    }
}
