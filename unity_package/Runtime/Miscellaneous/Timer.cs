using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using ErisJGDK.Base.UI.Animations;
using ErisJGDK.Base.Utils;

namespace ErisJGDK.Base.Miscellaneous
{
    public enum TimerFormat
    {
        [InspectorName("M:SS")] MSS,
        [InspectorName("MM:SS")] MMSS,
        TotalSeconds
    }

    public class Timer : MonoBehaviour
    {
        public static Timer Instance { get; private set; }

        [Header("General")]
        public float TimeLeft = 120f;
        public float TotalDuration = 180f;
        public bool Started;

        [Header("Events")]
        public UnityEvent OnTick;
        public UnityEvent OnTimeOver;

        [Header("Text")]
        [SerializeField] private TMP_Text _text;
        [SerializeField] private TimerFormat _timerFormat = TimerFormat.TotalSeconds;
        [SerializeField] private StringFormat.RoundMode _roundMode = StringFormat.RoundMode.Floor;

        [Header("Slider")]
        [SerializeField] private Slider _slider;

        [Header("Audio")]
        [SerializeField] private string _audioChannelName = "SFX";
        [SerializeField] private AudioClip _tickClip;

        [Space(5f)]

        [SerializeField] private float _lowTimeThreshold = 10f;
        [SerializeField] private AudioClip _lowTimeTickClip;

        private AudioChannel _audioChannel;
        private Animator _animator;
        private AnimatorHelper _animatorHelper;

        private float _previousValue;


        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            _audioChannel = AudioManager.Instance.GetChannel(_audioChannelName);
            _animator = GetComponent<Animator>();
            _animatorHelper = GetComponent<AnimatorHelper>();

            gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (!Started)
                return;

            _previousValue = TimeLeft;
            TimeLeft -= Time.fixedDeltaTime;
            float timeLeftClamped = Mathf.Max(0f, TimeLeft);

            if (_text != null)
                _text.text = StringFormat.FormatTime(timeLeftClamped, _timerFormat, _roundMode);

            if (_slider != null)
                _slider.value = timeLeftClamped / TotalDuration;

            if (StringFormat.RoundWithMode(TimeLeft, _roundMode) < StringFormat.RoundWithMode(_previousValue, _roundMode) && TimeLeft > 0f)
                Tick();

            if (TimeLeft < 0f)
            {
                TimeLeft = 0f;
                Started = false;
                OnTimeOver.Invoke();
            }
        }

        private void Tick()
        {
            if (_animator != null)
                _animator.SetTrigger("Tick");

            if (_audioChannel != null)
            {
                if (TimeLeft <= _lowTimeThreshold && _lowTimeTickClip != null)
                    AudioManager.Instance.Play(_audioChannel, _lowTimeTickClip);
                else if (_tickClip != null)
                    AudioManager.Instance.Play(_audioChannel, _tickClip);
            }

            OnTick.Invoke();
        }

        public void Show()
        {
            gameObject.SetActive(true);

            if (_animatorHelper != null)
                _animatorHelper.Show();
        }

        public void Hide()
        {
            if (_animatorHelper != null)
                _animatorHelper.Hide(() => gameObject.SetActive(false));
            else
                gameObject.SetActive(false);
        }

        public void SetDuration(float duration)
        {
            TotalDuration = duration;
            ResetTimer();
        }

        public void StartTimer() => Started = true;
        public void StopTimer() => Started = false;
        public void ResetTimer() => TimeLeft = TotalDuration;
    }
}
