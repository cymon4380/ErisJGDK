using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace ErisJGDK.Example
{
    public class TimelineTest : MonoBehaviour
    {
        private PlayableDirector _director;

        private void Start()
        {
            _director = GetComponent<PlayableDirector>();
            StartCoroutine(Play());
        }

        private IEnumerator Play()
        {
            yield return new WaitForSeconds(3f);
            _director.Play();

            while(_director.state == PlayState.Playing)
                yield return null;

            _director.Stop();
            yield return new WaitForSeconds(3f);
            _director.Play();
            yield return null;
            _director.Pause();

            yield return new WaitForSeconds(3f);
            _director.Play();
        }
    }
}