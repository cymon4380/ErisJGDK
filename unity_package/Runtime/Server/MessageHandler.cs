using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ErisJGDK.Base
{
    public class MessageHandler : MonoBehaviour
    {
        public static MessageHandler Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void HandleMessage(string message)
        {
            WSResponse response = JsonConvert.DeserializeObject<WSResponse>(message);

            if (response.error != null)
                Debug.LogError(response.error);

            Player player;
            PlayerInput playerInput;

            switch (response.key)
            {
                case "roomCreated":
                    Room room = new(
                        code: response.val["code"].ToString(),
                        audienceEnabled: Convert.ToBoolean(response.val["audienceEnabled"]),
                        minPlayers: Convert.ToInt32(response.val["minPlayers"]),
                        maxPlayers: Convert.ToInt32(response.val["maxPlayers"])
                    );

                    RoomManager.Instance.CurrentRoom = room;
                    RoomEvents.Instance.OnRoomCreated.Invoke(room);
                    break;

                case "playerConnected":
                    if (Convert.ToBoolean(response.val["reconnect"]))
                    {
                        player = RoomManager.Instance.CurrentRoom.GetPlayer(response.val["id"].ToString());
                        if (player == null)
                            return;

                        player.Disconnected = false;
                        RoomEvents.Instance.OnPlayerReconnected.Invoke(player);

                        if (player.LastInputs != null)
                            RoomManager.Instance.CurrentRoom.SendInputs(player.LastInputs, new[] { player });
                    }
                    else
                    {
                        player = new(response.val["name"].ToString(), response.val["id"].ToString(),
                            response.val["role"].ToString() == "player" ? Player.PlayerRole.Player : Player.PlayerRole.Audience);

                        if (!RoomManager.Instance.CurrentRoom.Locked || player.Role == Player.PlayerRole.Audience)
                        {
                            RoomManager.Instance.CurrentRoom.Players.Add(player);
                            RoomEvents.Instance.OnPlayerJoined.Invoke(player);
                        }
                    }

                    break;

                case "playerDisconnected":
                    player = RoomManager.Instance.CurrentRoom.GetPlayer(response.val["id"].ToString());
                    if (player == null)
                        return;

                    player.Disconnected = true;
                    if (player.Role == Player.PlayerRole.Audience)
                        RoomManager.Instance.CurrentRoom.Players.Remove(player);

                    RoomEvents.Instance.OnPlayerDisconnected.Invoke(player);
                    break;

                case "connectionClosed":
                    RoomEvents.Instance.OnRoomDestroyed.Invoke();
                    break;

                case "nameCensored":
                    player = RoomManager.Instance.CurrentRoom.GetPlayers(Player.PlayerRole.Player)
                        .FirstOrDefault(p => p.Name == response.val["name"].ToString());

                    if (player == null)
                        return;

                    player.NameCensored = true;
                    RoomEvents.Instance.OnPlayerNameCensored.Invoke(player);
                    break;

                case "playerKicked":
                    player = RoomManager.Instance.CurrentRoom.GetPlayer(response.val["id"].ToString());
                    if (player == null)
                        return;

                    if (!RoomManager.Instance.CurrentRoom.Locked)
                        RoomManager.Instance.CurrentRoom.Players.Remove(player);
                    else
                        player.Kicked = true;

                    RoomEvents.Instance.OnPlayerKicked.Invoke(player, response.val["reason"].ToString());
                    break;

                case "roomLocked":
                    RoomEvents.Instance.OnRoomLocked.Invoke();
                    break;

                case "input":
                    player = RoomManager.Instance.CurrentRoom.GetPlayer(response.val["playerId"].ToString());
                    if (player == null)
                        return;

                    var inputs = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.val["data"].ToString());

                    foreach(var input in inputs)
                    {
                        playerInput = new(input.Key, input.Value, Guid.NewGuid().ToString(), player, player.LastInputs);
                        RoomEvents.Instance.OnPlayerInput.Invoke(player, playerInput);
                    }

                    break;

                case "moderatorJoined":
                    RoomEvents.Instance.OnModeratorJoined.Invoke();
                    RoomManager.Instance.CurrentRoom.RestoreModeratorState(response.val["id"].ToString());
                    RoomManager.Instance.CurrentRoom.ModeratorsOnline = true;
                    break;

                case "moderatorDisconnected":
                    int moderatorCount = Convert.ToInt32(response.val["moderatorCount"]);
                    RoomEvents.Instance.OnModeratorDisconnected.Invoke(moderatorCount);
                    RoomManager.Instance.CurrentRoom.ModeratorsOnline = moderatorCount > 0;
                    break;

                case "inputModerated":
                    playerInput = RoomManager.Instance.CurrentRoom.GetInput(response.val["id"].ToString());
                    if (playerInput == null)
                        return;

                    switch(response.val["status"].ToString())
                    {
                        case "approved":
                            playerInput.Status = PlayerInput.ModerationStatus.Approved;
                            break;

                        case "rejected":
                            playerInput.Status = PlayerInput.ModerationStatus.Rejected;
                            break;

                        default:
                            return;
                    }

                    RoomEvents.Instance.OnInputModerated.Invoke(playerInput);
                    break;
            }
        }
    }
}