using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

namespace ErisJGDK.Base
{
    public class WebSocketClient : MonoBehaviour
    {
        public static WebSocketClient Instance { get; private set; }

        public WebSocket Client;
        private readonly ConcurrentQueue<Action> _actions = new();

        private bool _init = false;
        private bool _logMessages = false;
        private bool _wasOpen, _disconnected;

        [SerializeField] private float _pingInterval = 30f;  // Cloudflare disconnects WebSockets remaining dormant for 100 seconds. That fixes the issue.
        private float _untilNextPing;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            _untilNextPing = _pingInterval;
        }

        public void Init(string roomId)
        {
            Client = new(ConnectionSettings.GetString(false) + $"?id={roomId}");
            Client.OnMessage += OnMessage;
            Client.OnClose += OnClose;
            Client.OnOpen += OnOpen;

            _init = true;
        }

        public void Connect()
        {
            Client.ConnectAsync();
        }

        public void Disconnect()
        {
            _disconnected = true;
            Client.Close();
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            if (_logMessages)
                Debug.Log(e.Data);

            _actions.Enqueue(() => MessageHandler.Instance.HandleMessage(e.Data));
        }

        private void OnClose(object sender, EventArgs e)
        {
            RoomEvents.Instance.OnRoomDestroyed.Invoke();
        }

        private void OnOpen(object sender, EventArgs e)
        {
            _wasOpen = true;
        }

        private void Update()
        {
            while (_actions.Count > 0)
            {
                if (_actions.TryDequeue(out var action))
                {
                    action?.Invoke();
                }
            }
        }

        private void FixedUpdate()
        {
            if (Client == null)
                return;

            if (Client.ReadyState == WebSocketState.Open && _pingInterval > 0)
            {
                _untilNextPing -= Time.fixedDeltaTime;
                if (_untilNextPing <= 0)
                {
                    _untilNextPing = _pingInterval;
                    Client.Send(JsonConvert.SerializeObject(new Dictionary<string, string>
                    {
                        { "key", "ping" },
                        { "val", null }
                    }));
                }
            }

            if (Client.ReadyState == WebSocketState.Closed && _wasOpen && !_disconnected)
                RoomEvents.Instance.OnRoomDestroyed.Invoke();
        }

        private void OnDisable()
        {
            if (_init)
            {
                Client.OnMessage -= OnMessage;
                Client.OnClose -= OnClose;
                _init = false;
            }
        }
    }
}