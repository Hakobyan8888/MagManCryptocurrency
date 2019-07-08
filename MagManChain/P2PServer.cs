using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        /// <param name="e">Event argument</param>
        protected override void OnMessage(MessageEventArgs e)
        {
            if (JsonConvert.DeserializeObject(e.Data).ToString().Contains("Amount"))
            {
                Leader leader = new Leader();
                JObject jObject = JObject.Parse(e.Data);
                string fromAddress = "ha.stepan02@gmail.com";
                string toAddress = (string)jObject["ToAddress"];
                decimal amount = (decimal)jObject["Amount"];
                Transaction transaction = new Transaction(fromAddress, toAddress, amount);
                StartProgram.magMan.CreateTransaction(transaction);
                StartProgram.magMan.ProcessPendingTransactions(StartProgram.name);
            }
            else if (e.Data == "Hi Server")
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
