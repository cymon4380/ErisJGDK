using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ErisJGDK.Base.Utils;

namespace ErisJGDK.Base.UI
{
    public class PlayerCard : MonoBehaviour
    {
        public Player Player { get; private set; }

        [Header("General")]
        [SerializeField] private Image _avatarImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _scoreText;

        [SerializeField] private Color _generalAvatarColor = Color.white;
        [SerializeField] private Color _generalNameColor = Color.white;

        [Header("No Player")]
        [SerializeField] private Sprite _noPlayerAvatarSprite;
        [SerializeField] private Color _noPlayerAvatarColor = new(1f, 1f, 1f, .75f);
        [SerializeField] private Color _noPlayerNameColor = new(1f, 1f, 1f, .75f);
        [SerializeField] private string _noPlayerNameText = "JOIN!";

        [Header("Disconnected Player")]
        [SerializeField] private Color _disconnectedPlayerAvatarColor = new(1f, 1f, 1f, .75f);
        [SerializeField] private Color _disconnectedPlayerNameColor = new(1f, 1f, 1f, .75f);

        [Header("Censored Name")]
        [SerializeField] private Color _censoredNameColor = new(.8f, .075f, 0f, .85f);

        [Header("Kicked Player")]
        [SerializeField] private Color _kickedPlayerAvatarColor = new(1f, .7f, .7f, .5f);
        [SerializeField] private Color _kickedPlayerNameColor = new(1f, .7f, .7f, .5f);

        [Header("Score")]

        [Min(.01f)]
        [SerializeField] private float _scoreCountTime = 1.25f;

        private int _scoreStart, _scoreEnd;
        private float _scoreLerpTime = .6f;


        private void OnEnable()
        {
            RoomEvents.Instance.OnPlayerDisconnected.AddListener(UpdatePlayerDataEvent);
            RoomEvents.Instance.OnPlayerReconnected.AddListener(UpdatePlayerDataEvent);
            RoomEvents.Instance.OnPlayerNameCensored.AddListener(UpdatePlayerDataEvent);
            RoomEvents.Instance.OnPlayerKicked.AddListener(PlayerKicked);

            UpdatePlayerData();
        }

        private void OnDisable()
        {
            RoomEvents.Instance.OnPlayerDisconnected.RemoveListener(UpdatePlayerDataEvent);
            RoomEvents.Instance.OnPlayerReconnected.RemoveListener(UpdatePlayerDataEvent);
            RoomEvents.Instance.OnPlayerNameCensored.RemoveListener(UpdatePlayerDataEvent);
            RoomEvents.Instance.OnPlayerKicked.RemoveListener(PlayerKicked);
        }

        public void Assign(Player player, bool disableIfNotAssigned = false)
        {
            Player = player;

            if (disableIfNotAssigned)
                gameObject.SetActive(Player != null);
        }

        public void UpdatePlayerData()
        {
            if (Player == null)
            {
                if (_avatarImage != null)
                {
                    _avatarImage.sprite = _noPlayerAvatarSprite;
                    _avatarImage.color = _noPlayerAvatarColor;
                }

                if (_nameText != null)
                {
                    _nameText.color = _noPlayerNameColor;
                    _nameText.text = _noPlayerNameText;
                }

                return;
            }

            Color avatarColor, nameColor;

            if (Player.Kicked)
            {
                avatarColor = _kickedPlayerAvatarColor;
                nameColor = _kickedPlayerNameColor;
            }
            else if (Player.Disconnected)
            {
                avatarColor = _disconnectedPlayerAvatarColor;
                nameColor = _disconnectedPlayerNameColor;
            }
            else
            {
                avatarColor = _generalAvatarColor;
                nameColor = Player.NameCensored ? _censoredNameColor : _generalNameColor;
            }

            if (_avatarImage != null)
            {
                _avatarImage.color = avatarColor;
                _avatarImage.sprite = Player.Avatar;
            }

            if (_nameText != null)
            {
                _nameText.color = nameColor;
                _nameText.text = Player.DisplayName;
            }
        }

        public void UpdateScore(int? score = null)
        {
            score ??= Player == null ? 0 : Player.Score;

            if (_scoreText != null)
                _scoreText.text = StringFormat.SeparateThousands((int)score);
        }

        public void CountUpScore(int start, int end)
        {
            _scoreStart = start;
            _scoreEnd = end;
            _scoreLerpTime = 0f;
        }

        private void UpdatePlayerDataEvent(Player player)
        {
            if (player != Player)
                return;

            UpdatePlayerData();
        }

        private void PlayerKicked(Player player, string reason)
        {
            if (player != Player)
                return;

            if (!RoomManager.Instance.CurrentRoom.Players.Contains(player))
                Player = null;

            UpdatePlayerData();
        }

        private void FixedUpdate()
        {
            if (_scoreLerpTime >= 1)
                return;

            _scoreLerpTime += Time.fixedDeltaTime / _scoreCountTime;
            _scoreLerpTime = Mathf.Clamp(_scoreLerpTime, 0f, 1f);

            UpdateScore((int) Mathf.Lerp(_scoreStart, _scoreEnd, _scoreLerpTime));
        }
    }
}