using UnityEngine;
using UnityEngine.UI;

namespace ErisJGDK.Base.UI
{
    [RequireComponent(typeof(Button))]
    public class HideCodeButton : MonoBehaviour
    {
        [SerializeField] private RoomCodeText _roomCodeText;
        [SerializeField] private Image _image;

        [Header("Sprites")]
        [SerializeField] private Sprite _shownSprite;
        [SerializeField] private Sprite _hiddenSprite;

        private bool _shown;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Toggle);

            SetShown(!RoomSettings.HideCode, false);
        }

        public void Toggle()
        {
            SetShown(!_shown);
        }

        private void SetShown(bool shown, bool updateText = true)
        {
            _shown = shown;
            
            if (shown)
            {
                if (updateText)
                    _roomCodeText.Show();

                _image.sprite = _shownSprite;
            }
            else
            {
                if (updateText)
                    _roomCodeText.Hide();

                _image.sprite = _hiddenSprite;
            }
        }
    }
}