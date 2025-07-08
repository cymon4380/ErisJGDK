using UnityEngine;

namespace ErisJGDK.Base.UI
{
    public class PlayerList : MonoBehaviour
    {
        private PlayerCard[] _cards;

        [SerializeField] private bool _disableUnassignedCards = true;

        public PlayerCard[] Cards => _cards;

        private void Awake()
        {
            _cards = GetComponentsInChildren<PlayerCard>();
        }

        private void OnEnable()
        {
            AssignCards();
        }

        public void AssignCards()
        {
            var players = RoomManager.Instance.CurrentRoom.GetPlayers(Player.PlayerRole.Player);

            for (int i = 0; i < _cards.Length; i++)
            {
                Player player = i < players.Length ? players[i] : null;

                _cards[i].Assign(player, _disableUnassignedCards);
                _cards[i].UpdatePlayerData();
            }
        }
    }
}