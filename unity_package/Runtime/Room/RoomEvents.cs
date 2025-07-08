using UnityEngine;
using UnityEngine.Events;

namespace ErisJGDK.Base
{
    public class RoomEvents : MonoBehaviour
    {
        public static RoomEvents Instance { get; private set; }

        [Header("Player Events")]
        public UnityEvent<Player> OnPlayerJoined;
        public UnityEvent<Player> OnPlayerReconnected;
        public UnityEvent<Player> OnPlayerDisconnected;
        public UnityEvent<Player> OnPlayerNameCensored;
        public UnityEvent<Player, string> OnPlayerKicked;
        public UnityEvent<Player, PlayerInput> OnPlayerInput;

        [Header("Room Events")]
        public UnityEvent<Room> OnRoomCreated;
        public UnityEvent OnRoomLocked;
        public UnityEvent OnRoomDestroyed;

        [Header("Gameplay Events")]
        public UnityEvent OnGameStarted;
        public UnityEvent OnTimeUp;

        [Header("Moderator Events")]
        public UnityEvent OnModeratorJoined;
        public UnityEvent<int> OnModeratorDisconnected;
        public UnityEvent<PlayerInput> OnInputModerated;


        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
    }
}