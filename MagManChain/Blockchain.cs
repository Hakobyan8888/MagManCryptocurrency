using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace MagMan
{
    /// <summary>
    /// A sequence of blocks containing some sort of data
    /// </summary>
    public class Blockchain
    {
        private int i = 0;
        private Leader leader = new Leader(); // Leader who pays money to miners
        private string Connection { get; set; } = "Data Source=(local);Initial Catalog=Blockchain;Integrated Security=true"; // Connect to SQL database
        public IList<Transaction> PendingTransactions = new List<Transaction>(); // Stores newly added transactions.
        public IList<Block> Chain { set; get; } // Stores the main blockchain info
        public int Difficulty { set; get; } = 2; // Indicates the number of leading zeros required for a generated hash
        public decimal Reward = 1; // Money that leader pays to miners 

        /// <summary>
        /// Constructor
        /// </summary>
        public Blockchain()
        {
        }

        /// <summary>
        /// Initializing chain
        /// </summary>
        public void InitializeChain()
        {
            Chain = new List<Block>();
            AddGenesisBlock();
        }

        /// <summary>
        /// Creates first block
        /// </summary>
        public Block CreateGenesisBlock()
        {
            Block block = new Block(DateTime.Now, null, PendingTransactions);
            block.Mine(Difficulty);
            PendingTransactions = new List<Transaction>();
            return block;
        }

        /// <summary>
        /// Adds first block to a chain
        /// </summary>
        public void AddGenesisBlock()
        {
            Chain.Add(CreateGenesisBlock());
        }

        /// <summary>
        /// Gets last block 
        /// </summary>
        public Block GetLatestBlock()
        {
            return Chain[Chain.Count - 1];
        }

        /// <summary>
        /// Adds a new transaction to the PendingTransaction collection. 
        /// </summary>
        /// <param name="transaction"> New transaction </param>
        public void CreateTransaction(Transaction transaction)
        {
            PendingTransactions.Add(transaction);
        }

        /// <summary>
        /// Pending transactions are processed, the PendingTransactions field is reset and 
        /// then a new transaction is added to give the reward to the miner.
        /// </summary>
        /// <param name="minerAddress"> A miner address</param>
        public void ProcessPendingTransactions(string minerAddress)
        {
            Block block = new Block(DateTime.Now, GetLatestBlock().Hash, PendingTransactions);
            AddBlock(block);

            PendingTransactions = new List<Transaction>();
            CreateTransaction(new Transaction(leader.LeaderAddress, minerAddress, Reward));
        }

        /// <summary>
        /// Adds that block to a chain
        /// </summary>
        /// <param name="block"></param>
        public void AddBlock(Block block)
        {
            Block latestBlock = GetLatestBlock();

            block.Index = latestBlock.Index + 1;
            block.PreviousHash = latestBlock.Hash;            
            block.Mine(this.Difficulty);
            ActualizeTransactions(block);
            Chain.Add(block);
        }

        /// <summary>
        /// Actualize transactions
        /// </summary>
        /// <param name="block">The block where transactions are actualized</param>
        public void ActualizeTransactions(Block block)
        {
            using (SqlConnection Connection = new SqlConnection(this.Connection))
            {
                foreach (var transaction in block.Transactions)
                {
                    if (transaction.FromAddress != leader.LeaderAddress)
                    {
                        SqlCommand commandAddressFrom = new SqlCommand("select dbo.ValidateUserEmail(@addressFrom)", Connection);
                        commandAddressFrom.Parameters.Add("@addressFrom", SqlDbType.VarChar);
                        commandAddressFrom.Parameters["@addressFrom"].Value = transaction.FromAddress;

                        SqlCommand commandAddressTo = new SqlCommand("select dbo.ValidateUserEmail(@addressTo)", Connection);
                        commandAddressTo.Parameters.Add("@addressTo", SqlDbType.VarChar);
                        commandAddressTo.Parameters["@addressTo"].Value = transaction.ToAddress;

                        Connection.Open();

                        decimal balanceFrom = (decimal)commandAddressFrom.ExecuteScalar();
                        decimal balanceTo = (decimal)commandAddressTo.ExecuteScalar();

                        if (balanceFrom > transaction.Amount)
                        {
                            SqlCommand commandAdding = new SqlCommand("UPDATE Users SET Balance = Balance + @amountAdd Where Email = @addressTo", Connection);
                            commandAdding.Parameters.Add("@amountAdd", SqlDbType.Money);
                            commandAdding.Parameters["@amountAdd"].Value = transaction.Amount;
                            commandAdding.Parameters.Add("@addressTo", SqlDbType.VarChar);
                            commandAdding.Parameters["@addressTo"].Value = transaction.ToAddress;
                            commandAdding.ExecuteNonQuery();

                            SqlCommand commandSub = new SqlCommand("UPDATE Users SET Balance = Balance - @amountSub Where Email = @addressFrom", Connection);
                            commandSub.Parameters.Add("@amountSub", SqlDbType.Money);
                            commandSub.Parameters["@amountSub"].Value = transaction.Amount;
                            commandSub.Parameters.Add("@addressFrom", SqlDbType.VarChar);
                            commandSub.Parameters["@addressFrom"].Value = transaction.FromAddress;
                            commandSub.ExecuteNonQuery();
                        }
                        Connection.Close();
                    }
                    else
                    {
                        leader.LeaderAmount -= Reward;
                        SqlCommand commandReward = new SqlCommand("UPDATE Users SET Balance = Balance + @reward Where Email = @addressTo", Connection);
                        commandReward.Parameters.Add("@reward", SqlDbType.Money);
                        commandReward.Parameters["@reward"].Value = transaction.Amount;
                        commandReward.Parameters.Add("@addressTo", SqlDbType.VarChar);
                        commandReward.Parameters["@addressTo"].Value = transaction.ToAddress;
                        Connection.Open();
                        commandReward.ExecuteNonQuery();
                        Connection.Close();
                    }
                }
            }
        }
    }
}
