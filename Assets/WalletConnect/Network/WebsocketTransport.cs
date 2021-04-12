using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WalletConnectSharp.Events;
using WalletConnectSharp.Events.Request;
using WalletConnectSharp.Events.Response;
using WalletConnectSharp.Models;
using UnityEngine;
using NativeWebSocket;

namespace WalletConnectSharp.Network
{
    public class WebsocketTransport : ITransport
    {
        private bool opened = false;

        private WebSocket client;
        private EventDelegator _eventDelegator;

        public WebsocketTransport(EventDelegator eventDelegator)
        {
            this._eventDelegator = eventDelegator;
        }

        public void Dispose()
        {
            if (client != null)
                client.CancelConnection();
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<MessageReceivedEventArgs> OpenReceived;

        public async Task Open(string url)
        {
            if (url.StartsWith("https"))
                url = url.Replace("https", "wss");
            else if (url.StartsWith("http"))
                url = url.Replace("http", "ws");
            
            if (client != null)
                return;

            Debug.Log(url);

            //string testUrl = "ws://echo.websocket.org/";
            //string testUrl = "ws://localhost:8080";

            client = new WebSocket(url);

            client.OnOpen += () =>
            {
                Debug.Log("Connection Open!");

                // subscribe now
                if (this.OpenReceived != null)
                    OpenReceived(this, null);

            };

            client.OnMessage += (e) =>
            {
                OnMessageReceived(e);
            };

            //client.DisconnectionHappened.Subscribe(delegate(DisconnectionInfo info) { client.Reconnect(); });

            client.OnClose += (e) => {

                Debug.Log("OnClose " + e);
  
            };

            client.OnError += (e) => {

                Debug.Log("OnError " + e);

            };

            //client.ReconnectionHappened.Subscribe(delegate(ReconnectionInfo info)
            //{
            //   Console.WriteLine(info.Type);
            //});

            await client.Connect();
        }

        public void DispatchMessageQueue()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (opened)
            {
                client.DispatchMessageQueue();
            }
#endif
        }

        private void OnMessageReceived(byte[] bytes)
        {
            string json = System.Text.Encoding.UTF8.GetString(bytes); ;

            Debug.Log("OnMessageReceived length: " + json.Length);
            Debug.Log("OnMessageReceived data: " + json);

            try
            {
                var msg = JsonConvert.DeserializeObject<NetworkMessage>(json);

                client.SendText(new NetworkMessage()
                {
                    Payload = "",
                    Type = "ack",
                    Silent = true,
                    Topic = msg.Topic
                }.ToString());

                if (this.MessageReceived != null)
                    MessageReceived(this, new MessageReceivedEventArgs(msg, this));
            }
            catch(Exception e)
            {
                Debug.Log("Exception " + e.Message);
            }   
        }

        public async Task Close()
        {
            await client.Close();
        }

        public async Task SendMessage(NetworkMessage message)
        {
            string finalJson = JsonConvert.SerializeObject(message);
            
            await this.client.SendText(finalJson);
        }

        public async Task Subscribe(string topic)
        {
            Debug.Log("Subscribe");

            await SendMessage(new NetworkMessage()
            {
                Payload = "",
                Type = "sub",
                Silent = true,
                Topic = topic
            });

            opened = true;

        }

        public async Task Subscribe<T>(string topic, EventHandler<JsonRpcResponseEvent<T>> callback) where T : JsonRpcResponse
        {
            await Subscribe(topic);

            _eventDelegator.ListenFor(topic, callback);
        }
        
        public async Task Subscribe<T>(string topic, EventHandler<JsonRpcRequestEvent<T>> callback) where T : JsonRpcRequest
        {
            await Subscribe(topic);

            _eventDelegator.ListenFor(topic, callback);
        }
    }
}