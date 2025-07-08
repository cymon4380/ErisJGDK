using System;
using System.Collections.Generic;
using System.Linq;
using ErisJGDK.Base.Extensions;
using Newtonsoft.Json;

namespace ErisJGDK.Base
{
    public class Room
    {
        public string Code { get; private set; }
        public bool AudienceEnabled { get; private set; }

        public string Password { get; private set; }
        public string ModerationPassword { get; private set; }

        public int MinPlayers { get; private set; }
        public int MaxPlayers { get; private set; }

        public bool ModeratorsOnline = false;
        public bool Locked = false;
        public int CurrentRound = 1;

        public List<Player> Players = new();
        public List<PlayerInput> PlayerInputs = new();

        public Room(
            string code,
            bool audienceEnabled,
            int minPlayers, int maxPlayers,
            string password = null, string moderationPassword = null
        )
        {
            Code = code;
            AudienceEnabled = audienceEnabled;
            Password = password;
            ModerationPassword = moderationPassword;
            MinPlayers = minPlayers;
            MaxPlayers = maxPlayers;
        }

        public Player GetPlayer(string id)
        {
            return Players.FirstOrDefault(p => p.Id == id);
        }

        public Player[] GetPlayers(Player.PlayerRole? role = null)
        {
            if (role == null)
                return Players.ToArray();

            return Players.Where(p => p.Role == role).ToArray();
        }

        /// <summary>
        /// Sends JSON data to certain players.
        /// </summary>
        /// <param name="recipients">Target recipients</param>
        /// <param name="ignore">Ignored recipients (override)</param>
        /// <param name="inputs">Inputs that will apply to recipients who are not ignored</param>
        public void SendData(
            Dictionary<string, object> data,
            Player[] recipients,
            Player[] ignore = null,
            Inputs inputs = null
        )
        {
            ignore = ignore.ArrayOrEmpty();
            WebSocketClient.Instance.Client.Send(JsonConvert.SerializeObject(data));

            if (inputs != null && recipients != null)
            {
                foreach (Player recipient in recipients)
                {
                    if (ignore.Contains(recipient))
                        continue;

                    recipient.LastInputs = inputs;
                }
            }
        }

        /// <summary>
        /// For server communication only!
        /// Gets either IDs of provided recipients or a flag that is used to send data to all players.
        /// </summary>
        /// <param name="recipientType">Whose IDs to get</param>
        /// <param name="players">Must be provided to get certain IDs</param>
        public static string[] GetRecipientIDs(Inputs.RecipientType recipientType = Inputs.RecipientType.CERTAIN,
            Player[] players = null)
        {
            if (recipientType != Inputs.RecipientType.CERTAIN)
            {
                string[] uuidList = new[]
                {
                    recipientType.ToString()
                };

                return uuidList;
            }

            return players.Select(p => p.Id).ToArray();
        }

        /// <summary>
        /// Gets an array of players to send data to.
        /// </summary>
        /// <param name="recipientType">Whom to include</param>
        /// <param name="players">Included players. Works only if recipientType is set to CERTAIN.</param>
        /// <param name="ignore">Ignored players. Works with all recipient types.</param>
        /// <exception cref="ArgumentException">If recipientType is CERTAIN, you must provide an array of players.</exception>
        public Player[] GetRecipients(Inputs.RecipientType recipientType = Inputs.RecipientType.CERTAIN,
            Player[] players = null, Player[] ignore = null)
        {
            if (recipientType == Inputs.RecipientType.CERTAIN && players == null)
                throw new ArgumentException("Player array must be provided");

            ignore = ignore.ArrayOrEmpty();
            players = players.ArrayOrEmpty();

            return recipientType switch
            {
                Inputs.RecipientType.ALL_PLAYERS => GetPlayers(Player.PlayerRole.Player)
                .Where(p => !ignore.Contains(p)).ToArray(),

                Inputs.RecipientType.ALL_AUDIENCE => GetPlayers(Player.PlayerRole.Audience)
                .Where(p => !ignore.Contains(p)).ToArray(),

                Inputs.RecipientType.CERTAIN => players.Where(p => !ignore.Contains(p)).ToArray(),
                _ => GetPlayers().Where(p => !ignore.Contains(p)).ToArray()
            };
        }

        /// <summary>
        /// Sends provided inputs to players.
        /// </summary>
        /// <param name="recipients">Players that will receive the inputs. Works only if recipientType is set to CERTAIN.</param>
        /// <param name="ignore">Players that will NOT receive the inputs. Works with all recipient types.</param>
        /// <param name="recipientType">Who will receive the inputs</param>
        public void SendInputs(Inputs inputs,
            Player[] recipients = null,
            Player[] ignore = null,
            Inputs.RecipientType recipientType = Inputs.RecipientType.CERTAIN)
        {
            ignore = ignore.ArrayOrEmpty();

            Dictionary<string, object> data = new()
            {
                { "key", "input" },
                { "val", new Dictionary<string, object>() {
                    { "prompt", inputs.Prompt },
                    { "submitButton", inputs.SubmitButton },
                    { "inputs", inputs.Serialize() }
                }},
                { "recipients", GetRecipientIDs(recipientType, recipients) },
                { "ignore", ignore.Select(i => i.Id) },
            };

            SendData(data, GetRecipients(recipientType, recipients, ignore), ignore, inputs);
        }

        /// <summary>
        /// Shows the logo to players.
        /// </summary>
        /// <param name="recipients">Players that will see the logo. Works only if recipientType is set to CERTAIN.</param>
        /// <param name="ignore">Players that will NOT see the logo. Works with all recipient types.</param>
        /// <param name="recipientType">Who will receive the logo</param>
        public void ShowLogo(
            Player[] recipients = null,
            Player[] ignore = null,
            Inputs.RecipientType recipientType = Inputs.RecipientType.CERTAIN)
        {
            ignore = ignore.ArrayOrEmpty();

            Dictionary<string, object> data = new()
            {
                { "key", "logo" },
                { "val", null },
                { "recipients", GetRecipientIDs(recipientType, recipients) },
                { "ignore", ignore.Select(i => i.Id) },
            };

            SendData(data, GetRecipients(recipientType, recipients, ignore), ignore);
        }

        /// <summary>
        /// Discards inputs so they cannot be replied to any longer.
        /// </summary>
        /// <param name="recipients">Whose inputs to drop</param>
        /// <param name="ignore">Whose inputs to keep (overrides recipients)</param>
        public void DropInputs(Player[] recipients, Player[] ignore = null)
        {
            ignore = ignore.ArrayOrEmpty();

            ShowLogo(recipients, ignore);

            foreach (Player recipient in recipients)
            {
                if (ignore.Contains(recipient))
                    continue;

                recipient.LastInputs = null;
            }
        }

        /// <summary>
        /// Locks the room. New players will not be able to join it.
        /// </summary>
        public void Lock()
        {
            Locked = true;
            WebSocketClient.Instance.Client.Send(JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                { "key", "lockRoom" },
                { "val", null }
            }));
        }

        /// <summary>
        /// Gets a player input. You must provide either input ID or a player and the key of their input.
        /// </summary>
        /// <exception cref="ArgumentException">Neither input ID nor player and input key were provided.</exception>
        public PlayerInput GetInput(string id = null, Player player = null, string key = null)
        {
            if (id != null)
                return PlayerInputs.Find(i => i.Id == id);
            if (player == null && key == null)
                throw new ArgumentException("You must specify both player and key");

            return PlayerInputs.Find(i => i.Player == player && i.Key == key);
        }

        /// <summary>
        /// Sends the data about players and inputs to a moderator.
        /// </summary>
        public void RestoreModeratorState(string moderatorId)
        {
            List<Dictionary<string, object>> players = new();
            List<Dictionary<string, object>> inputs = new();

            foreach(Player player in GetPlayers(Player.PlayerRole.Player))
            {
                players.Add(new()
                {
                    { "name", player.Name },
                    { "nameCensored", player.NameCensored },
                    { "kicked", player.Kicked }
                });
            }

            foreach(PlayerInput input in PlayerInputs)
            {
                if (!input.CanBeModerated() || input.Dropped)
                    continue;

                inputs.Add(input.Serialize());
            }

            Dictionary<string, object> data = new()
            {
                { "key", "welcomeRestoreState" },
                { "val", new Dictionary<string, object>
                    {
                        { "id", moderatorId },
                        { "players", players },
                        { "inputs", inputs }
                    }
                }
            };

            WebSocketClient.Instance.Client.Send(JsonConvert.SerializeObject(data));
        }
    }
}