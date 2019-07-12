using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WebSocketSharp;

namespace MagMan
{
    /// <summary>
    /// P2P Client is used to initializing a connection with a server via WebSocket.
    /// </summary>
    public class P2PClient
    {
        IDictionary<string, WebSocket> wsDict = new Dictionary<string, WebSocket>();

        public void Connect(string url)
        {
            if (!wsDict.ContainsKey(url))
            {
                var webSocket = new WebSocket(url);
                webSocket.OnMessage += (sender, e) =>
                {
                    if (e.Data == "Hi Client")
                    {
                        Console.WriteLine(e.Data);
                    }
                    else
                    {
                        var newChain = JsonConvert.DeserializeObject<Blockchain>(e.Data);
                        if (newChain.IsValid() && newChain.Chain.Count > StartProgram.MagMan.Chain.Count)
                        {
                            var newTransactions = new List<Transaction>();
                            newTransactions.AddRange(newChain.PendingTransactions);
                            newTransactions.AddRange(StartProgram.MagMan.PendingTransactions);

                            newChain.PendingTransactions = newTransactions;
                            StartProgram.MagMan = newChain;
                        }
                    }
                };
                webSocket.Connect();
                webSocket.Send("Hi Server");
                webSocket.Send(JsonConvert.SerializeObject(StartProgram.MagMan));
                wsDict.Add(url, webSocket);
            }
        }

        public void Send(string url, string data)
        {
            foreach (var item in wsDict)
            {
                if (item.Key == url)
                {
                    item.Value.Send(data);
                }
            }
        }

        public void Broadcast(string data)
        {
            foreach (var item in wsDict)
            {
                item.Value.Send(data);
            }
        }

        public IList<string> GetServers()
        {
            var servers = new List<string>();
            foreach (var item in wsDict)
            {
                servers.Add(item.Key);
            }
            return servers;
        }

        public void Close()
        {
            foreach (var item in wsDict)
            {
                item.Value.Close();
            }
        }
    }
}
