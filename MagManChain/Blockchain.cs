using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace MagMan
{
    public class Blockchain
    {
        private Leader leader = new Leader();
        public string ConnectionString { get; set; } = "Data Source=(local);Initial Catalog=Blockchain;Integrated Security=true";
        public IList<Transaction> PendingTransactions = new List<Transaction>(); // Store newly added transactions.
        public IList<Block> Chain { set; get; }

        public int Difficulty { set; get; } = 2;
        public decimal Reward = 1; //1 cryptocurrency

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
        /// Create first block
        /// </summary>
        public Block CreateGenesisBlock()
        {
            Block block = new Block(DateTime.Now, null, PendingTransactions);
            block.Mine(Difficulty);
            PendingTransactions = new List<Transaction>();
            return block;
        }

        /// <summary>
        /// Add first block to a chain
        /// </summary>
        public void AddGenesisBlock()
        {
            Chain.Add(CreateGenesisBlock());
        }

        /// <summary>
        /// Get last block 
        /// </summary>
        public Block GetLatestBlock()
        {
            return Chain[Chain.Count - 1];
        }

        /// <summary>
        /// Add a new transaction to the PendingTransaction collection. 
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
        /// Add that block to a chain
        /// </summary>
        /// <param name="block"></param>
        public void AddBlock(Block block)
        {
            Block latestBlock = GetLatestBlock();
            block.Index = latestBlock.Index + 1;
            block.PreviousHash = latestBlock.Hash;
            //block.Hash = block.CalculateHash();

            block.Mine(this.Difficulty);
            ActualizeTransactions(block);
            Chain.Add(block);
        }

        /// <summary>
        /// Calculate the balance of a user of the blockchain.
        /// </summary>
        /// <param name="address"> User address</param>
        /// <returns></returns>

        public void ActualizeTransactions(Block block)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {

                foreach (var transaction in block.Transactions)
                {
                    if (transaction.FromAddress != leader.LeaderAddress)
                    {
                        SqlCommand cmd1 = new SqlCommand("select dbo.ValidateUserEmail(@addressFrom)", con);
                        cmd1.Parameters.Add("@addressFrom", SqlDbType.VarChar);
                        cmd1.Parameters["@addressFrom"].Value = transaction.FromAddress;

                        SqlCommand cmd2 = new SqlCommand("select dbo.ValidateUserEmail(@addressTo)", con);
                        cmd2.Parameters.Add("@addressTo", SqlDbType.VarChar);
                        cmd2.Parameters["@addressTo"].Value = transaction.ToAddress;

                        con.Open();
                        decimal balanceFrom = (decimal)cmd1.ExecuteScalar();
                        decimal balanceTo = (decimal)cmd2.ExecuteScalar();

                        if (balanceFrom > transaction.Amount)
                        {
                            SqlCommand commandAdding = new SqlCommand("UPDATE Users SET Balance = Balance + @amountAdd Where Email = @addressTo", con);
                            commandAdding.Parameters.Add("@amountAdd", SqlDbType.Money);
                            commandAdding.Parameters["@amountAdd"].Value = transaction.Amount;
                            commandAdding.Parameters.Add("@addressTo", SqlDbType.VarChar);
                            commandAdding.Parameters["@addressTo"].Value = transaction.ToAddress;
                            commandAdding.ExecuteNonQuery();

                            SqlCommand commandSub = new SqlCommand("UPDATE Users SET Balance = Balance - @amountSub Where Email = @addressFrom", con);
                            commandSub.Parameters.Add("@amountSub", SqlDbType.Money);
                            commandSub.Parameters["@amountSub"].Value = transaction.Amount;
                            commandSub.Parameters.Add("@addressFrom", SqlDbType.VarChar);
                            commandSub.Parameters["@addressFrom"].Value = transaction.FromAddress;
                            commandSub.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    else
                    {
                        
                        leader.LeaderAmount -= Reward;
                        SqlCommand commandReward = new SqlCommand("UPDATE Users SET Balance = Balance + @reward Where Email = @addressTo", con);
                        commandReward.Parameters.Add("@reward", SqlDbType.Money);
                        commandReward.Parameters["@reward"].Value = transaction.Amount;
                        commandReward.Parameters.Add("@addressTo", SqlDbType.VarChar);
                        commandReward.Parameters["@addressTo"].Value = transaction.ToAddress;
                        con.Open();
                        commandReward.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
        }
    }
}