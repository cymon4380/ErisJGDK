using ErisJGDK.Base.UI;
using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using WebSocketSharp;

namespace ErisJGDK.Base
{
    public class RoomManager : MonoBehaviour
    {
        public static RoomManager Instance;

        public Room CurrentRoom;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Start()
        {
            RoomEvents.Instance.OnRoomDestroyed.AddListener(OnRoomDestroyed);
        }

        private void OnDestroy()
        {
            RoomEvents.Instance.OnRoomDestroyed.RemoveListener(OnRoomDestroyed);
        }

        public void CreateRoom(App app)
        {
            RoomSettings.Load();
            StartCoroutine(SendRequest(app));
        }

        private IEnumerator SendRequest(App app)
        {
            WWWForm form = new();
            form.AddField("appTag", app.Tag);
            form.AddField("audienceEnabled", RoomSettings.AudienceEnabled.ToString());
            form.AddField("password", RoomSettings.Password);
            form.AddField("moderationPassword", RoomSettings.ModerationPassword);

            UnityWebRequest uwr = UnityWebRequest.Post(ConnectionSettings.GetString(true), form);
            if (!ConnectionSettings.IsSecuredConnection)
                uwr.certificateHandler = new BypassCertificate();

            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.ConnectionError)
            {
                HttpResponse response = JsonConvert.DeserializeObject<HttpResponse>(uwr.downloadHandler.text);

                if (!response.ok)
                    throw new Exception("Error code: " + response.httpCode);

                ActivateRoom(response.body["roomId"].ToString());
            }
            else
            {
                throw new Exception(uwr.error);
            }
        }

        private void ActivateRoom(string roomId)
        {
            WebSocketClient.Instance.Init(roomId);
            WebSocketClient.Instance.Connect();
        }

        public void LockRoom()
        {
            CurrentRoom.Lock();
        }

        public void StartGame()
        {
            if (CurrentRoom.GetPlayers(Player.PlayerRole.Player).Length < CurrentRoom.MinPlayers)
                return;
            if (CurrentRoom.Locked)
                return;

            Countdown.Instance.StartCountdown(LockRoom);
        }

        public void ResetGame()
        {
            if (WebSocketClient.Instance != null)
                if (WebSocketClient.Instance.Client != null)
                    if (WebSocketClient.Instance.Client.ReadyState == WebSocketState.Open)
                        WebSocketClient.Instance.Client.Close();

            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnRoomDestroyed()
        {
            if (CurrentRoom == null || !CurrentRoom.Locked)
                ResetGame();
        }
    }
}