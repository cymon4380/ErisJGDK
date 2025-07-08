using System;
using System.Collections;
using UnityEngine;

namespace ErisJGDK.Base.UI.Animations
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorHelper : MonoBehaviour
    {
        private Animator _animator;

        public Animator Animator => _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public bool HasParameter(string paramName)
        {
            foreach (AnimatorControllerParameter param in _animator.parameters)
            {
                if (param.name == paramName)
                    return true;
            }

            return false;
        }

        public void Show(Action onShow = null)
        {
            if (HasParameter("Show"))
                _animator.SetTrigger("Show");
            else if (HasParameter("Shown"))
                _animator.SetBool("Shown", true);

            if (onShow != null)
                StartCoroutine(InvokeAfterPlaying(onShow));
        }

        public void Hide(Action onHide = null)
        {
            if (HasParameter("Hide"))
                _animator.SetTrigger("Hide");
            else if (HasParameter("Shown"))
                _animator.SetBool("Shown", false);

            if (onHide != null && gameObject.activeSelf)
                StartCoroutine(InvokeAfterPlaying(onHide));
        }

        public IEnumerator InvokeAfterPlaying(Action action, int layer = 0)
        {
            yield return null;

            while (_animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1
                || _animator.IsInTransition(layer))
                yield return null;

            action.Invoke();
        }
    }
}