using System.Linq;
using TMPro;
using UnityEngine;

namespace ErisJGDK.Base.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class RoomCodeText : MonoBehaviour
    {
        [SerializeField] private bool _canBeHidden;
        [SerializeField] private bool _autoUpdate = true;

        private TMP_Text _text;

        private void Awake()
        {
            CacheText();
        }

        private void OnEnable()
        {
            UpdateCode();

            if (_autoUpdate)
                RoomEvents.Instance.OnRoomCreated.AddListener(OnRoomCreated);
        }

        private void OnDisable()
        {
            if (_autoUpdate)
                RoomEvents.Instance.OnRoomCreated.RemoveListener(OnRoomCreated);
        }

        private void OnRoomCreated(Room room)
        {
            if (_autoUpdate)
                UpdateCode();
        }

        private void CacheText()
        {
            if (_text == null)
                _text = GetComponent<TMP_Text>();
        }

        public void UpdateCode()
        {
            Room room = RoomManager.Instance.CurrentRoom;

            if (room != null)
            {
                CacheText();

                if (_canBeHidden && RoomSettings.HideCode)
                    Hide();
                else
                    Show();
            }
        }

        public void Show()
        {
            _text.text = RoomManager.Instance.CurrentRoom.Code;
        }

        public void Hide()
        {
            _text.text = string.Concat(Enumerable.Repeat('*', RoomManager.Instance.CurrentRoom.Code.Length));
        }
    }
}
