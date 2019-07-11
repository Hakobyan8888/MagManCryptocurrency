using System;
using System.Collections.Generic;
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
        bool chainSynched = false;
        WebSocketServer wss = null;
        P2PClient Client = new P2PClient();

        public void Start()
        {
            wss = new WebSocketServer($"ws://127.0.0.1:{StartProgram.Port}");
            wss.AddWebSocketService<P2PServer>("/Blockchain");
            wss.Start();
            Console.WriteLine($"Started server at ws://127.0.0.1:{StartProgram.Port}");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.Data == "Hi Server")
            {
                Console.WriteLine(e.Data);
                Send("Hi Client");
            }
            else if (JsonConvert.DeserializeObject(e.Data).ToString().Contains("Type"))
            {
                Leader leader = new Leader();
                JObject jObject = JObject.Parse(e.Data);
                string fromAddress = leader.LeaderAddress;
                string toAddress = (string)jObject["ToAddress"];
                decimal amount = (decimal)jObject["Amount"];
                Transaction transaction = new Transaction(fromAddress, toAddress, amount);
                StartProgram.MagMan.CreateTransaction(transaction);
                StartProgram.MagMan.ProcessPendingTransactions(StartProgram.name);
                
            }
            else
            {
                Blockchain newChain = JsonConvert.DeserializeObject<Blockchain>(e.Data);

                if (newChain.IsValid() && newChain.Chain.Count > StartProgram.MagMan.Chain.Count)
                {
                    List<Transaction> newTransactions = new List<Transaction>();
                    newTransactions.AddRange(newChain.PendingTransactions);
                    newTransactions.AddRange(StartProgram.MagMan.PendingTransactions);

                    newChain.PendingTransactions = newTransactions;
                    StartProgram.MagMan = newChain;
                }

                if (!chainSynched)
                {
                    Send(JsonConvert.SerializeObject(StartProgram.MagMan));
                    chainSynched = true;
                }
            }
        }
    }
}
