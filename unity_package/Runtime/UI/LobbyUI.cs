using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using ErisJGDK.Base.Miscellaneous;
using ErisJGDK.Base.UI.Animations;

namespace ErisJGDK.Base.UI
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private RoomCodeText _roomCodeText;

        [Space(5)]

        [SerializeField] private PlayerCard[] _playerCards;
        [SerializeField] private bool _disableUnassignedCards;

        [Space(5)]

        [SerializeField] private TMP_Text _playerCountText;
        [SerializeField] private Button _startGameButton;

        private void OnEnable()
        {
            RoomEvents.Instance.OnPlayerJoined.AddListener(PlayerJoined);
            RoomEvents.Instance.OnPlayerKicked.AddListener(PlayerKicked);
        }

        private void OnDisable()
        {
            RoomEvents.Instance.OnPlayerJoined.RemoveListener(PlayerJoined);
            RoomEvents.Instance.OnPlayerKicked.RemoveListener(PlayerKicked);
        }

        private void PlayerJoined(Player player)
        {
            if (player.Role == Player.PlayerRole.Player)
            {
                PlayerCard card = _playerCards.First(c => c.Player == null);

                if (AvatarManager.Instance != null)
                {
                    PlayerIdentity identity = AvatarManager.Instance.GetAvailableAvatar();
                    if (identity != null)
                    {
                        player.Avatar = identity.Sprite;
                        player.Identity = identity;

                        AudioClip clip = identity.GetRandomSound();
                        if (clip != null)
                            AudioManager.Instance.GetChannel("Voice").Play(clip);
                    }
                }

                card.Assign(player, _disableUnassignedCards);
                card.UpdatePlayerData();

                UpdateStartSection();
            }
        }

        private void PlayerKicked(Player player, string reason)
        {
            UpdateStartSection();
        }

        public void UpdateStartSection()
        {
            int playerCount = RoomManager.Instance.CurrentRoom.GetPlayers(Player.PlayerRole.Player).Length;
            int minPlayers = RoomManager.Instance.CurrentRoom.MinPlayers;

            if (_playerCountText != null)
                _playerCountText.text = playerCount + "/" + minPlayers;


            AnimatorHelper buttonHelper = _startGameButton.GetComponent<AnimatorHelper>();
            bool buttonActive = playerCount >= minPlayers
                && !RoomManager.Instance.CurrentRoom.Locked;

            _startGameButton.interactable = buttonActive;

            if (buttonHelper != null)
            {
                if (buttonActive)
                    buttonHelper.Show();
                else
                    buttonHelper.Hide();
            }

            if (playerCount == minPlayers)
                _startGameButton.Select();
        }

        public void UpdateRoomCode()
        {
            if (_roomCodeText != null)
                _roomCodeText.UpdateCode();
        }
    }
}
