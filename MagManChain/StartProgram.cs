using System;
using Newtonsoft.Json;

namespace MagMan
{
    public class StartProgram
    {
        public static int Port = 0;
        public static P2PServer Server = null;
        public static P2PClient Client = new P2PClient();
        public static Blockchain magMan = new Blockchain();
        public static string name = "Unknown";

        public void Start(string[] args)
        {
            magMan.InitializeChain();

            if (args.Length >= 1)
            {
                Port = int.Parse(args[0]);
            }

            if (args.Length >= 2)
            {
                name = args[1];
            }

            if (Port > 0)
            {
                Server = new P2PServer();
                Server.Start();
            }
            if (name != "Unkown")
            {
                Console.WriteLine($"Current user is {name}");
            }

            Console.WriteLine("=========================");
            Console.WriteLine("1. Connect to a server");
            Console.WriteLine("2. Add a transaction");
            Console.WriteLine("3. Display Blockchain");
            Console.WriteLine("4. Exit");
            Console.WriteLine("=========================");

            int selection = 0;
            while (selection != 4)
            {
                switch (selection)
                {
                    case 1:
                        Console.WriteLine("Please enter the server URL");
                        string serverURL = Console.ReadLine();
                        Client.Connect($"{serverURL}/Blockchain");
                        break;
                    case 2:
                        Console.WriteLine("Please enter the receiver name");
                        string receiverName = Console.ReadLine();
                        Console.WriteLine("Please enter the amount");
                        string amount = Console.ReadLine();
                        magMan.CreateTransaction(new Transaction(name, receiverName, decimal.Parse(amount)));
                        magMan.ProcessPendingTransactions(name);
                        Client.Broadcast(JsonConvert.SerializeObject(magMan));
                        break;
                    case 3:
                        Console.WriteLine("Blockchain");
                        Console.WriteLine(JsonConvert.SerializeObject(magMan, Formatting.Indented));
                        break;

                }

                Console.WriteLine("Please select an action");
                string action = Console.ReadLine();
                selection = int.Parse(action);
            }

            Client.Close();
        }
    }
}
