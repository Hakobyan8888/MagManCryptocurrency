using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace MagMan
{
    /// <summary>
    /// P2P Server is used to listen for client connections via WebSocket.
    /// </summary>
    public class P2PServer : WebSocketBehavior
    {
        private bool chainSynched = false;
        private WebSocketServer webSocketServer = null;

        /// <summary>
        /// Established a connection with a client
        /// </summary>
        public void Start()
        {
            webSocketServer = new WebSocketServer($"ws://127.0.0.1:{StartProgram.Port}");
            webSocketServer.AddWebSocketService<P2PServer>("/Blockchain");
            webSocketServer.Start();
            Console.WriteLine($"Started server at ws://127.0.0.1:{StartProgram.Port}");
        }

        /// <summary> 
        /// The server verifies data and compares it with its own Blockchain. 
        /// If the client blockchain is valid and it is longer than the server Blockchain, 
        /// the server uses the client blockchain, otherwise, 
        /// the server will send a copy of its own Blockchain to the client.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.Data == "Hi Server")
            {
                Console.WriteLine(e.Data);
                Send("Hi Client");
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

                if (!chainSynched)
                {
                    Send(JsonConvert.SerializeObject(StartProgram.magMan.PendingTransactions));
                    Send(JsonConvert.SerializeObject(StartProgram.magMan));
                    chainSynched = true;
                }
            }
        }
    }
}
