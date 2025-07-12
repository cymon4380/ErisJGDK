using System;
using System.Linq;
using UnityEngine;

namespace ErisJGDK.Base
{
    public class AvatarManager : MonoBehaviour
    {
        public static AvatarManager Instance;

        [SerializeField] private PlayerIdentity[] _avatars;

        public PlayerIdentity[] Avatars => _avatars;


        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public PlayerIdentity GetAvailableAvatar()
        {
            return _avatars.OrderBy(_ => new Guid()).FirstOrDefault(a => !IsAvatarUsed(a));
        }

        private bool IsAvatarUsed(PlayerIdentity avatar)
        {
            return RoomManager.Instance.CurrentRoom.GetPlayers(Player.PlayerRole.Player)
                .Any(p => p.Identity == avatar);
        }
    }
}
