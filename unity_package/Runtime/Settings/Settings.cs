using ErisJGDK.Base.Miscellaneous;
using ErisJGDK.Base.UI.Animations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ErisJGDK.Base.Settings
{
    public interface ISetting
    {
        public void Load();
        public void Save();
        public void Apply();
    }

    public class Settings : MonoBehaviour
    {
        public static Settings Instance { get; private set; }

        [SerializeField] private GameObject _overlay;
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private CanvasGroup[] _disabledOnRoomCreated;

        private ISetting[] _settings;

        private AnimatorHelper _animatorHelper;
        private bool _shown;
        private bool _overlayWasActive;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            _animatorHelper = _settingsPanel.GetComponent<AnimatorHelper>();
            _settings = GetComponentsInChildren<ISetting>();
        }

        private void Start()
        {
            foreach (var setting in _settings)
            {
                setting.Load();
                setting.Apply();
            }
        }

        public void Show()
        {
            if (_shown)
                return;

            _shown = true;

            if (_overlay != null)
            { 
                _overlayWasActive = _overlay.activeSelf;
                _overlay.SetActive(true);
            }

            if (_canvasGroup != null)
                _canvasGroup.interactable = true;

            if (RoomManager.Instance.CurrentRoom != null)
                foreach(var canvasGroup in _disabledOnRoomCreated)
                    canvasGroup.interactable = false;

            _settingsPanel.SetActive(true);

            if (_animatorHelper != null)
            _animatorHelper.Show();

            Pause.Instance.CanPause = false;
            EventSystem.current.SetSelectedGameObject(null);
        }

        public void Hide()
        {
            if (!_shown)
                return;

            _shown = false;

            if (_canvasGroup != null)
                _canvasGroup.interactable = false;

            HidePanel();
        }

        public void SaveAndApply()
        {
            foreach (var setting in _settings)
            {
                setting.Save();
                setting.Apply();
            }
        }

        private void HidePanel()
        {
            _settingsPanel.SetActive(false);
            Pause.Instance.CanPause = true;

            if (!_overlayWasActive && _overlay != null)
                _overlay.SetActive(false);
        }
    }
}