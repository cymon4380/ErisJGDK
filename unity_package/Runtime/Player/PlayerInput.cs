using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ErisJGDK.Base
{
    public class PlayerInput
    {
        public enum ModerationStatus
        {
            Pending,
            Approved,
            Rejected
        }

        private readonly string _key;
        private readonly object _value;
        private readonly string _id;
        private readonly Player _player;
        private readonly Inputs _inputs;

        public string Key => _key;
        public object Value => _value;
        public string Id => _id;
        public Player Player => _player;
        public Inputs Inputs => _inputs;

        public ModerationStatus Status = ModerationStatus.Pending;
        public bool Dropped = false;

        public PlayerInput(string key, object value, string id, Player player, Inputs inputs)
        {
            _key = key;
            _value = value;
            _id = id;
            _player = player;
            _inputs = inputs;
        }

        public bool CanBeModerated()
        {
            if (Inputs == null)
                return false;

            InputElement inputElement = Inputs.Elements.FirstOrDefault(e => e.Key == _key);
            if (inputElement == null)
                return false;

            return inputElement is InputText;
        }

        /// <summary>
        /// Serializes the input to send it to moderation as JSON.
        /// </summary>
        public Dictionary<string, object> Serialize()
        {
            Dictionary<string, object> data = new()
            {
                { "id", Id },
                { "status", Status.ToString().ToLower() },
                { "playerName", Player.Name },
                { "content", Value }
            };

            return data;
        }

        /// <summary>
        /// Sends the input to moderation.
        /// </summary>
        public void Moderate()
        {
            if (!CanBeModerated() || Dropped)
                return;

            WebSocketClient.Instance.Client.Send(JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                { "key", "moderateInput" },
                { "val", Serialize() }
            }));
        }

        /// <summary>
        /// Makes the input ineligible for moderation.
        /// </summary>
        public void Drop()
        {
            if (!CanBeModerated() || Dropped)
                return;

            Dropped = true;
            WebSocketClient.Instance.Client.Send(JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                { "key", "dropInput" },
                { "val", new Dictionary<string, object>
                    {
                        { "id", Id }
                    }
                }
            }));
        }
    }
}
