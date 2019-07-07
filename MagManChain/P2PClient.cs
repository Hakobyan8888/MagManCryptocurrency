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
        private IDictionary<string, WebSocket> webSocketDictionary = new Dictionary<string, WebSocket>(); // Represents data and protocol pair

        /// <summary>
        /// Connect to the server
        /// </summary>
        /// <param name="url"> Specified URL</param>
        public void Connect(string url)
        {
            if (!webSocketDictionary.ContainsKey(url))
            {
                WebSocket webSocketClient = new WebSocket(url);
                webSocketClient.OnMessage += (sender, e) =>
                {
                    if (e.Data == "Hi Client")
                    {
                        Console.WriteLine(e.Data);
                    }
                    else
                    {
                        Blockchain newChain = JsonConvert.DeserializeObject<Blockchain>(e.Data);
                        if (newChain.IsValid() && newChain.Chain.Count > StartProgram.magMan.Chain.Count)
                        {
                            List<Transaction> newTransactions = new List<Transaction>();
                            newTransactions.AddRange(newChain.PendingTransactions);
                            newTransactions.AddRange(StartProgram.magMan.PendingTransactions);

                            newChain.PendingTransactions = newTransactions;
                            StartProgram.magMan = newChain;
                        }
                        if (newChain.IsValid() && newChain.PendingTransactions.Count > StartProgram.magMan.PendingTransactions.Count)
                        {
                            StartProgram.magMan.PendingTransactions = newChain.PendingTransactions;
                        }
                    }
                };
                webSocketClient.Connect();
                webSocketClient.Send("Hi Server");
                webSocketClient.Send(JsonConvert.SerializeObject(StartProgram.magMan.PendingTransactions));
                webSocketClient.Send(JsonConvert.SerializeObject(StartProgram.magMan));
                webSocketDictionary.Add(url, webSocketClient);
            }
        }

        /// <summary>
        /// Sends the data using connection
        /// </summary>
        /// <param name="data"></param>
        public void Broadcast(string data)
        {
            foreach (var item in webSocketDictionary)
            {
                item.Value.Send(data);
            }
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        public void Close()
        {
            foreach (var item in webSocketDictionary)
            {
                item.Value.Close();
            }
        }
    }
}
