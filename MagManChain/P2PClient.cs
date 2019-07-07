using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WebSocketSharp;

namespace MagMan
{
    public class P2PClient
    {
        IDictionary<string, WebSocket> wsDict = new Dictionary<string, WebSocket>();

        public void Connect(string url)
        {
            if (!wsDict.ContainsKey(url))
            {
                WebSocket ws = new WebSocket(url);
                ws.OnMessage += (sender, e) =>
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
                ws.Connect();
                ws.Send("Hi Server");
                ws.Send(JsonConvert.SerializeObject(StartProgram.magMan.PendingTransactions));
                ws.Send(JsonConvert.SerializeObject(StartProgram.magMan));
                wsDict.Add(url, ws);
            }
        }

        public void Broadcast(string data)
        {
            foreach (var item in wsDict)
            {
                item.Value.Send(data);
            }
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
