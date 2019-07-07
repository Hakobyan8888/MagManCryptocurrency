using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace MagMan
{
    public class P2PServer : WebSocketBehavior
    {
        bool chainSynched = false;
        WebSocketServer wss = null;

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
