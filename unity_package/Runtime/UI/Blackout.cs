using System;
using ErisJGDK.Base.UI.Animations;
using UnityEngine;

namespace ErisJGDK.Base.UI
{
    public class Blackout : MonoBehaviour
    {
        public static Blackout Instance { get; private set; }

        private AnimatorHelper _animatorHelper;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            _animatorHelper = GetComponent<AnimatorHelper>();
        }

        public void Show(Action onShow = null)
        {
            _animatorHelper.Show(onShow);
        }

        public void Hide(Action onHide = null)
        {
            _animatorHelper.Hide(onHide);
        }
    }
}