using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace ErisJGDK.Base.UI.Animations
{
    [Serializable]
    public class NamedTimeline
    {
        public string Name;
        public TimelineAsset Asset;
    }

    [RequireComponent(typeof(PlayableDirector))]
    public class TimelineHelper : MonoBehaviour
    {
        private PlayableDirector _director;
        [SerializeField] private NamedTimeline[] _timelines;

        public PlayableDirector Director => _director;


        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
            GoToFirstFrame();
        }

        public void GoToFirstFrame() => StartCoroutine(ToFirstFrame());

        public void Play(Action onFinishPlaying = null)
        {
            _director.Play();

            if (onFinishPlaying != null)
                StartCoroutine(InvokeAfterPlaying(onFinishPlaying));
        }

        public void PlayTimeline(string name, Action onFinishPlaying = null)
        {
            NamedTimeline timeline = GetTimeline(name);
            if (timeline == null)
                throw new ArgumentException("Timeline not found");

            _director.playableAsset = timeline?.Asset;
            _director.Play();

            if (onFinishPlaying != null)
                StartCoroutine(InvokeAfterPlaying(onFinishPlaying));
        }

        public NamedTimeline GetTimeline(string name)
        {
            return _timelines.FirstOrDefault(t => t.Name == name);
        }

        private IEnumerator ToFirstFrame()
        {
            _director.time = 0f;
            _director.Play();
            yield return null;
            _director.Pause();
        }

        private IEnumerator InvokeAfterPlaying(Action action)
        {
            yield return null;

            while (_director.state == PlayState.Playing)
                yield return null;

            action.Invoke();
        }
    }
}