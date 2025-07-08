using System;
using System.Collections;
using ErisJGDK.Base.Miscellaneous;
using ErisJGDK.Base.UI.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ErisJGDK.Base.UI
{
    public class Countdown : MonoBehaviour
    {
        public static Countdown Instance { get; private set; }

        private TMP_Text _text;
        private AnimatorHelper _animatorHelper;
        private AudioChannel _audioChannel;

        private bool _isStarted = false;

        [Min(0f)]
        [SerializeField] private int _countdownDuration = 3;
        [SerializeField] private bool _splitAudioClips;

        [SerializeField] private Button _startGameButton;

        public UnityEvent OnCountdownFinished;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            _text = GetComponent<TMP_Text>();
            _animatorHelper = GetComponent<AnimatorHelper>();
            _audioChannel = AudioManager.Instance.GetChannel("SFX");
        }

        private void OnEnable()
        {
            RoomEvents.Instance.OnPlayerJoined.AddListener(OnPlayerJoined);
            RoomEvents.Instance.OnPlayerKicked.AddListener(OnPlayerKicked);
        }

        private void OnDisable()
        {
            RoomEvents.Instance.OnPlayerJoined.RemoveListener(OnPlayerJoined);
            RoomEvents.Instance.OnPlayerKicked.RemoveListener(OnPlayerKicked);
        }

        public void StartCountdown(Action onFinished = null)
        {
            StartCoroutine(StartCountDown(onFinished));
        }

        public void StopCountdown()
        {
            if (!_isStarted)
                return;

            _isStarted = false;
            StopCoroutine(StartCountDown());

            if (_animatorHelper != null)
                _animatorHelper.Hide();

            _startGameButton.interactable = true;

            _audioChannel?.AudioSource.Stop();
        }

        private void OnPlayerJoined(Player player)
        {
            if (player.Role != Player.PlayerRole.Player)
                return;

            StopCountdown();
        }

        private void OnPlayerKicked(Player player, string reason)
        {
            StopCountdown();
        }

        private IEnumerator StartCountDown(Action onFinished = null)
        {
            if (_isStarted)
                yield break;

            _isStarted = true;
            _startGameButton.interactable = false;

            if (_audioChannel != null && !_splitAudioClips)
                _audioChannel.Play("Countdown");

            if (_animatorHelper != null)
                _animatorHelper.Show();

            for (int i = _countdownDuration; i > 0; i--)
            {
                if (!_isStarted)
                    yield break;

                _text.text = i.ToString();
                if (_audioChannel != null && _splitAudioClips)
                    _audioChannel.Play($"Countdown{i}");

                yield return new WaitForSeconds(1f);
            }

            if (!_isStarted)
                yield break;

            _isStarted = false;
            OnCountdownFinished.Invoke();
            onFinished?.Invoke();
        }
    }
}