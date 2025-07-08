using UnityEngine;
using TMPro;

namespace ErisJGDK.Base.UI
{
    public class AudienceCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private bool _hideIfAudienceDisabled = true;

        private void Awake()
        {
            if (_text == null)
                _text = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            UpdateAudienceCount();

            RoomEvents.Instance.OnPlayerJoined.AddListener(PlayerJoined);
            RoomEvents.Instance.OnPlayerDisconnected.AddListener(PlayerDisconnected);
        }

        private void OnDisable()
        {
            RoomEvents.Instance.OnPlayerJoined.RemoveListener(PlayerJoined);
            RoomEvents.Instance.OnPlayerDisconnected.RemoveListener(PlayerDisconnected);
        }

        public void UpdateAudienceCount()
        {
            int audienceCount = RoomManager.Instance.CurrentRoom.GetPlayers(Player.PlayerRole.Audience).Length;

            _text.text = audienceCount.ToString();

            if (_hideIfAudienceDisabled)
                gameObject.SetActive(RoomManager.Instance.CurrentRoom.AudienceEnabled);
        }

        private void PlayerJoined(Player player)
        {
            if (player.Role != Player.PlayerRole.Audience)
                return;

            UpdateAudienceCount();
        }

        private void PlayerDisconnected(Player player)
        {
            if (player.Role != Player.PlayerRole.Audience)
                return;

            UpdateAudienceCount();
        }
    }
}