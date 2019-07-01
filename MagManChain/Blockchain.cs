using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace MagMan
{
    public class Blockchain
    {
        public string ConnectionString { get; set; } = "Data Source=(local);Initial Catalog=Blockchain;Integrated Security=true";
        public IList<Transaction> PendingTransactions = new List<Transaction>(); // Store newly added transactions.
        public IList<Block> Chain { set; get; }
        public int Difficulty { set; get; } = 2; 
        public int Reward = 1; //1 cryptocurrency

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
            CreateTransaction(new Transaction(null, minerAddress, Reward));
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
                decimal balanceFrom;
                decimal balanceTo;

                foreach (var transaction in block.Transactions)
                {
                    SqlCommand cmd1 = new SqlCommand("select dbo.User ValidateUserEmail(@code)", con);
                    SqlParameter param1 = new SqlParameter("@code", SqlDbType.VarChar);
                    param1.Value = transaction.FromAddress;
                    con.Open();
                    balanceFrom = (decimal)cmd1.ExecuteScalar();

                    SqlCommand cmd2 = new SqlCommand("select dbo.User ValidateUserEmail(@code)", con);
                    SqlParameter param2 = new SqlParameter("@code", SqlDbType.VarChar);
                    param2.Value = transaction.ToAddress;
                    balanceTo = (decimal)cmd2.ExecuteScalar();
                    con.Close();

                    if (balanceFrom > transaction.Amount)
                    {

                    }

                }
            }

        }

    }
}