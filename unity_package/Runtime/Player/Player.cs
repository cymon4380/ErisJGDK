using System.Linq;
using UnityEngine;

namespace ErisJGDK.Base
{
    public class Player
    {
        public enum PlayerRole
        {
            Player,
            Audience
        }

        public string Name { get; private set; }
        public string Id { get; private set; }
        public PlayerRole Role { get; private set; }
        public string DisplayName
        {
            get
            {
                return NameCensored ? string.Concat(Enumerable.Repeat("*", Name.Length)) : Name;
            }
        }

        public Inputs LastInputs = null;
        public bool Disconnected = false;
        public bool NameCensored = false;
        public bool Kicked = false;

        public Sprite Avatar;
        public PlayerIdentity Identity;

        public int Score = 0;
        public int OldScore = 0;

        public Player(string name, string id, PlayerRole role)
        {
            Name = name.Replace("\n", string.Empty);
            Id = id;
            Role = role;
        }
    }
}