using ErisJGDK.Base.UI.Animations;
using UnityEngine;

namespace ErisJGDK.Base.Miscellaneous
{
    public class Pause : MonoBehaviour
    {
        public static Pause Instance { get; private set; }

        [SerializeField] private GameObject _overlay;
        [SerializeField] private GameObject _menu;
        [SerializeField] private CanvasGroup _canvasGroup;

        public bool IsPaused { get; private set; }
        public bool CanPause = true;

        private AnimatorHelper _animatorHelper;
        private AudioChannel[] _audioChannels;

        private float _initialTimeScale = 1f;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            _animatorHelper = _menu.GetComponent<AnimatorHelper>();
        }

        private void Start()
        {
            _audioChannels = AudioManager.Instance.Channels;
        }

        public void PauseGame()
        {
            if (!CanPause || IsPaused)
                return;

            IsPaused = true;
            _initialTimeScale = Time.timeScale;
            Time.timeScale = 0f;

            if (_overlay != null)
                _overlay.SetActive(true);

            if (_animatorHelper != null)
                _animatorHelper.Show();

            _menu.SetActive(true);

            if (_canvasGroup != null)
                _canvasGroup.interactable = true;

            foreach(var channel in _audioChannels)
                channel.Pause();
        }

        public void UnPauseGame()
        {
            if (!CanPause || !IsPaused)
                return;

            if (_canvasGroup != null)
                _canvasGroup.interactable = false;

            if (_animatorHelper != null)
                _animatorHelper.Hide(UnPause);
            else
                UnPause();
        }

        public void ResetGame()
        {
            Time.timeScale = 1f;
            RoomManager.Instance.ResetGame();
        }

        private void UnPause()
        {
            IsPaused = false;
            Time.timeScale = _initialTimeScale;

            if (_overlay != null)
                _overlay.SetActive(false);

            _menu.SetActive(false);

            foreach (var channel in _audioChannels)
                channel.UnPause();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (IsPaused)
                    UnPauseGame();
                else
                    PauseGame();
            }
        }
    }
}
