using UnityEngine;

namespace ErisJGDK.Base
{
    [CreateAssetMenu(fileName = "PlayerIdentity", menuName = "Player Identity")]
    public class PlayerIdentity : ScriptableObject
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private AudioClip[] _sounds;

        public Sprite Sprite => _sprite;
        public AudioClip[] Sounds => _sounds;

        public AudioClip GetRandomSound()
        {
            if (_sounds.Length == 0)
                return null;

            return _sounds[new System.Random().Next(_sounds.Length)];
        }
    }
}