using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ErisJGDK.Base.Settings
{
    public class RoomSettingsManager : MonoBehaviour, ISetting
    {
        [SerializeField] private TMP_InputField _roomPasswordField;
        [SerializeField] private TMP_InputField _moderationPasswordField;

        [SerializeField] private Toggle _audienceToggle;
        [SerializeField] private Toggle _hideCodeToggle;
        [SerializeField] private Toggle _skipTutorialToggle;

        public void Load()
        {
            RoomSettings.Load();

            _roomPasswordField.text = RoomSettings.Password;
            _moderationPasswordField.text = RoomSettings.ModerationPassword;
            _audienceToggle.isOn = RoomSettings.AudienceEnabled;
            _hideCodeToggle.isOn = RoomSettings.HideCode;
            _skipTutorialToggle.isOn = RoomSettings.SkipTutorial;
        }

        public void Save()
        {
            RoomSettings.Password = _roomPasswordField.text;
            RoomSettings.ModerationPassword = _moderationPasswordField.text;
            RoomSettings.AudienceEnabled = _audienceToggle.isOn;
            RoomSettings.HideCode = _hideCodeToggle.isOn;
            RoomSettings.SkipTutorial = _skipTutorialToggle.isOn;

            RoomSettings.Save();
        }

        public void Apply() {  }
    }
}