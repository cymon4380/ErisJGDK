using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ErisJGDK.Base.Miscellaneous
{
    [Serializable]
    public class AudioChannel
    {
        public string Name;

        public string AudioClipsPath;
        public AudioSource AudioSource;
        public List<AudioClip> Queue = new();
        public float Volume = .5f;

        public bool IsPaused => AudioManager.Instance.PausedChannels.Contains(this);

        /// <summary>
        /// Returns true if the channel is playing, paused, or the game is paused.
        /// Is used for sequential playback.
        /// </summary>
        public bool IsBusy => AudioSource.isPlaying || IsPaused || Miscellaneous.Pause.Instance.IsPaused;

        public AudioChannel(string audioClipPath, AudioSource audioSource)
        {
            AudioClipsPath = audioClipPath;
            AudioSource = audioSource;
        }

        /// <summary>
        /// Sets a provided or stored volume of the channel.
        /// </summary>
        public void SetVolume(float? volume = null)
        {
            if (volume != null)
                AudioSource.volume = (float)volume;
            else
                AudioSource.volume = Volume;
        }

        /// <summary>
        /// Gets an audio clip by name in the channel's path.
        /// </summary>
        public AudioClip GetAudioClip(string name)
        {
            return AudioManager.Instance.GetAudioClip(this, name);
        }

        /// <summary>
        /// Gets a random audio clip with prefix in the channel's path.
        /// </summary>
        public AudioClip GetRandomAudioClip(string prefix)
        {
            return AudioManager.Instance.GetRandomAudioClip(this, prefix);
        }

        /// <summary>
        /// Plays a looped clip. You must specify either its name in the channel's path or a clip itself.
        /// </summary>
        public void PlayLooped(string clipName = null, AudioClip clip = null)
        {
            AudioManager.Instance.PlayLooped(this, clipName, clip);
        }

        /// <summary>
        /// Plays a specified clip.
        /// </summary>
        /// <param name="onPlayed">Action that will be invoked after playing</param>
        /// <param name="waitForEndPlaying">Play after the previous clip ends playing</param>
        public void Play(AudioClip clip, Action onPlayed = null, bool waitForEndPlaying = false)
        {
            AudioManager.Instance.Play(this, clip, onPlayed, waitForEndPlaying);
        }

        /// <summary>
        /// Plays a clip by its name in the channel's path.
        /// </summary>
        /// <param name="onPlayed">Action that will be invoked after playing</param>
        /// <param name="waitForEndPlaying">Play after the previous clip ends playing</param>
        public void Play(string clipName, Action onPlayed = null, bool waitForEndPlaying = false)
        {
            Play(GetAudioClip(clipName), onPlayed, waitForEndPlaying);
        }

        /// <summary>
        /// Plays a random audio clip with prefix in the channel's path.
        /// </summary>
        /// <param name="onPlayed">Action that will be invoked after playing</param>
        /// <param name="waitForEndPlaying">Play after the previous clip ends playing</param>
        public void PlayRandom(string prefix, Action onPlayed = null, bool waitForEndPlaying = false)
        {
            Play(GetRandomAudioClip(prefix), onPlayed, waitForEndPlaying);
        }

        /// <summary>
        /// Starts playing all clips in the channel's queue.
        /// </summary>
        /// <param name="onFinished">Action that will be invoked after playing all clips</param>
        public void PlayQueue(Action onFinished = null)
        {
            AudioManager.Instance.PlayChannelQueue(this, onFinished);
        }

        /// <summary>
        /// Gradually increases the channel's volume to the stored one within specified time.
        /// </summary>
        /// <param name="duration">Duration in seconds</param>
        public void FadeIn(float duration)
        {
            AudioManager.Instance.FadeIn(this, duration);
        }

        /// <summary>
        /// Gradually decreases the channel's volume to the stored one within specified time.
        /// </summary>
        /// <param name="duration">Duration in seconds</param>
        public void FadeOut(float duration, bool stop = true)
        {
            AudioManager.Instance.FadeOut(this, duration, stop);
        }

        /// <summary>
        /// Pauses playback.
        /// </summary>
        public void Pause()
        {
            AudioManager.Instance.PausedChannels.Add(this);
            AudioSource.Pause();
        }

        /// <summary>
        /// Resumes playback.
        /// </summary>
        public void UnPause()
        {
            AudioManager.Instance.PausedChannels.Remove(this);
            AudioSource.UnPause();
        }
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [SerializeField] private string _audioRootPath;

        [Space(5f)]

        [SerializeField] private AudioChannel[] _channels;

        public readonly HashSet<AudioChannel> PausedChannels = new();
        public AudioChannel[] Channels => _channels;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            foreach (AudioChannel channel in _channels)
            {
                channel.Volume = PlayerPrefs.GetFloat("Volume_" + channel.Name, .5f);
                channel.SetVolume();
            }
        }

        /// <summary>
        /// Gets the path to a clip by name in the channel's path.
        /// </summary>
        public string GetClipPath(AudioChannel channel, string clipName)
        {
            return _audioRootPath + channel.AudioClipsPath + clipName;
        }

        /// <summary>
        /// Gets a clip by name in the channel's path.
        /// </summary>
        public AudioClip GetAudioClip(AudioChannel channel, string clipName)
        {
            return Resources.Load<AudioClip>(GetClipPath(channel, clipName));
        }

        /// <summary>
        /// Gets a random clip by prefix in the channel's path.
        /// </summary>
        public AudioClip GetRandomAudioClip(AudioChannel channel, string prefix)
        {
            AudioClip[] clips = Resources.LoadAll<AudioClip>(_audioRootPath + channel.AudioClipsPath)
                .Where(c => c.name.StartsWith(prefix)).ToArray();

            return clips[new System.Random().Next(clips.Length)];
        }

        /// <summary>
        /// Plays a looped clip by the channel. You must specify either clip name in the channel's path or a clip itself.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void PlayLooped(AudioChannel channel, string clipName = null, AudioClip clip = null)
        {
            if (clipName == null && clip == null)
                throw new ArgumentException("You must specify either clip name or a clip itself");

            channel.AudioSource.clip = clip != null ? clip : GetAudioClip(channel, clipName);
            channel.AudioSource.loop = true;
            channel.AudioSource.Play();
        }

        /// <summary>
        /// Plays a clip by the channel.
        /// </summary>
        /// <param name="onPlayed">Action that will be invoked after playing</param>
        /// <param name="waitForEndPlaying">Play after the previous clip ends playing</param>
        public void Play(AudioChannel channel, AudioClip clip, Action onPlayed = null, bool waitForEndPlaying = false)
        {
            if (waitForEndPlaying)
            {
                StartCoroutine(WaitForEndPlaying(channel, clip, onPlayed));
                return;
            }

            channel.AudioSource.PlayOneShot(clip);
            if (onPlayed != null)
                StartCoroutine(InvokeAfterPlaying(channel, onPlayed));
        }

        /// <summary>
        /// Gets an audio channel by name.
        /// </summary>
        public AudioChannel GetChannel(string name)
        {
            return _channels.FirstOrDefault(c => c.Name.ToLower() == name.ToLower());
        }

        /// <summary>
        /// Plays the channel's queue.
        /// </summary>
        /// <param name="onFinished">Action that will be invoked after playing all clips</param>
        public void PlayChannelQueue(AudioChannel channel, Action onFinished = null)
        {
            StartCoroutine(PlayQueue(channel, onFinished));
        }

        /// <summary>
        /// Plays the channel's queue.
        /// </summary>
        /// <param name="onFinished">Action that will be invoked after playing all clips</param>
        public IEnumerator PlayQueue(AudioChannel channel, Action onFinished = null)
        {
            foreach (AudioClip clip in channel.Queue)
            {
                channel.AudioSource.PlayOneShot(clip);
                yield return null;

                yield return new WaitUntil(() => !channel.IsBusy);
            }

            channel.Queue.Clear();
            onFinished?.Invoke();
        }

        /// <summary>
        /// Gradually increases the channel's volume to the stored one within specified time.
        /// </summary>
        /// <param name="duration">Duration in seconds</param>
        public void FadeIn(AudioChannel channel, float duration)
        {
            StartCoroutine(LerpVolume(channel, 0, channel.Volume, duration));
        }

        /// <summary>
        /// Gradually decreases the channel's volume to the stored one within specified time.
        /// </summary>
        /// <param name="duration">Duration in seconds</param>
        public void FadeOut(AudioChannel channel, float duration, bool stop)
        {
            StartCoroutine(LerpVolume(channel, channel.Volume, 0, duration, stop ? (() => channel.AudioSource.Stop()) : null));
        }

        private IEnumerator LerpVolume(AudioChannel channel, float startVolume, float endVolume, float duration, Action onFinish = null)
        {
            float lerpTime = 0;

            while (lerpTime < 1)
            {
                lerpTime += Time.fixedDeltaTime / duration;
                lerpTime = Mathf.Clamp01(lerpTime);

                channel.AudioSource.volume = Mathf.Lerp(startVolume, endVolume, lerpTime);
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }

            onFinish?.Invoke();
        }

        private IEnumerator InvokeAfterPlaying(AudioChannel channel, Action action)
        {
            yield return null;
            yield return new WaitUntil(() => !channel.IsBusy);

            action.Invoke();
        }

        public IEnumerator WaitForEndPlaying(AudioChannel channel, AudioClip clip, Action onPlayed = null)
        {
            yield return null;
            yield return new WaitUntil(() => !channel.IsBusy);

            Play(channel, clip, onPlayed);
        }
    }
}
