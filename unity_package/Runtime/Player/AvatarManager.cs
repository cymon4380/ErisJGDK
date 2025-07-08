using System.Collections.Generic;
using UnityEngine;

namespace ErisJGDK.Base
{
    public class AvatarManager : MonoBehaviour
    {
        public static AvatarManager Instance;

        [SerializeField] private List<PlayerIdentity> _availableAvatars;

        public List<PlayerIdentity> AvailableAvatars => _availableAvatars;


        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public PlayerIdentity GetAvailableAvatar()
        {
            if (_availableAvatars.Count == 0)
                return null;

            PlayerIdentity identity = _availableAvatars[new System.Random().Next(_availableAvatars.Count)];
            _availableAvatars.Remove(identity);

            return identity;
        }
    }
}
