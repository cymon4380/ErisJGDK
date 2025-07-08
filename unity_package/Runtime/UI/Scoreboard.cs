using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;
using ErisJGDK.Base.UI.Animations;
using System;

namespace ErisJGDK.Base.UI
{
    public class Scoreboard : MonoBehaviour
    {
        public enum OrderType
        {
            SiblingIndex,
            Points
        }

        public static Scoreboard Instance { get; private set; }

        [SerializeField] private AnimatorHelper _animatorHelper;

        [SerializeField] private OrderType _orderType;
        [SerializeField] private PlayerCard[] _playerCards;

        [Space(5f)]
        [SerializeField] private TMP_Text _titleText;

        [Header("Order by points")]

        [SerializeField] private Transform[] _cardPoints;
        [SerializeField] private float _orderDuration = .75f;

        private Player[] _players;
        private float _lerpTime = 1f;
        private Dictionary<PlayerCard, Transform> _cardsLerpTargets = new();
        private bool _isInit;

        public bool HasNoPoints => _players.Any(p => p.Score == 0);

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            if (_animatorHelper == null)
                _animatorHelper = GetComponentInChildren<AnimatorHelper>();

            gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (_lerpTime < 1f)
            {
                _lerpTime += Time.fixedDeltaTime / _orderDuration;

                foreach(var pair in _cardsLerpTargets)
                    pair.Key.transform.position = Vector2.Lerp(pair.Key.transform.position, pair.Value.position,
                        _lerpTime);
            }
        }

        public void Init(Player[] players)
        {
            if (_isInit)
                return;

            _players = players;
            _isInit = true;

            for (int i = 0; i < _playerCards.Length; i++)
            {
                PlayerCard card = _playerCards[i];

                card.Assign(i < _players.Length ? _players[i] : null, true);
                card.UpdatePlayerData();
                card.UpdateScore();
            }
        }

        public void CountUpScore()
        {
            foreach (PlayerCard card in _playerCards)
            {
                if (card.Player == null)
                    continue;

                card.CountUpScore(card.Player.OldScore, card.Player.Score);
                card.Player.OldScore = card.Player.Score;
            }
        }

        public Player[] GetWinners()
        {
            int highestScore = _players.OrderByDescending(p => p.Score).First().Score;
            return _players.Where(p => p.Score ==  highestScore).ToArray();
        }

        public void OrderPlayers()
        {
            Player[] orderedPlayers = _players.OrderByDescending(p => p.Score).ToArray();
            _cardsLerpTargets.Clear();

            if (_orderType == OrderType.SiblingIndex)
            {
                foreach (PlayerCard card in _playerCards)
                {
                    if (card.Player == null)
                        continue;

                    card.transform.SetSiblingIndex(Array.IndexOf(orderedPlayers, card.Player));
                }
            }
            else
            {
                foreach (PlayerCard card in _playerCards)
                {
                    if (card.Player == null)
                        continue;

                    _cardsLerpTargets[card] = _cardPoints[Array.IndexOf(orderedPlayers, card.Player)];
                }

                _lerpTime = 0f;
            }
        }

        public void SetTitle(string text) => _titleText.text = text;
        public void Show() => gameObject.SetActive(true);
        public void Hide() => _animatorHelper.Hide(() => gameObject.SetActive(false));
    }
}
