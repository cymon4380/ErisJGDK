using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ErisJGDK.Base.UI
{
    [RequireComponent(typeof(Button))]
    public class CopyCodeButton : MonoBehaviour
    {
        [SerializeField] private Image _image;

        [Header("Sprites")]
        [SerializeField] private Sprite _copySprite;
        [SerializeField] private Sprite _copiedSprite;

        [Header("Properties")]
        [SerializeField] private float _copiedSpriteDuration = 2f;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Copy);
        }

        public void Copy()
        {
            GUIUtility.systemCopyBuffer = RoomManager.Instance.CurrentRoom.Code;

            _image.sprite = _copiedSprite;
            _button.interactable = false;

            StartCoroutine(ResetSprite());
        }

        private IEnumerator ResetSprite()
        {
            yield return new WaitForSeconds(_copiedSpriteDuration);

            _image.sprite = _copySprite;
            _button.interactable = true;
        }
    }
}