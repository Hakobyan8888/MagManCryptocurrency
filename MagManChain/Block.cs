using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace MagMan
{
    /// <summary>
    /// Transaction data is permanently recorded in files called blocks. 
    /// </summary>
    public class Block : IBlock
    {
        public int Index { get; set; } // Symbolizes the location of the block inside the blockchain.
        public DateTime TimeStamp { get; set; } // Saves the time aspects of when the block was built.
        public string Hash { get; set; } // Fixed length number derived from a given message or document.
        public string PreviousHash { get; set; } // Hash of a neighbnor node.        
        public IList<Transaction> Transactions { get; set; }  // A set of messages or transactions.
        public int Nonce { get; set; } // Saves the integer (32 or 64bits) that are utilized in the mining method.

        /// <summary>
        /// Initializing
        /// </summary>
        public Block(DateTime timeStamp, string previousHash, IList<Transaction> transactions)
        {
            Index = 0;
            TimeStamp = timeStamp;
            PreviousHash = previousHash;
            Transactions = transactions;
            Hash = CalculateHash();
            Nonce = 0;
        }

        /// <summary>
        /// Generating hash
        /// </summary>
        /// <returns> Hash </returns>
        public string CalculateHash()
        {
            SHA256 sha256 = SHA256.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes($"{TimeStamp}-{PreviousHash ?? ""}-{JsonConvert.SerializeObject(Transactions)}-{Nonce}");
            byte[] outputBytes = sha256.ComputeHash(inputBytes);

            return Convert.ToBase64String(outputBytes);
        }

        /// <summary>
        /// Tries to find a hash that matches with difficulty. If a generated hash doesn’t meet the difficulty, 
        /// then it increases nonce to generate a new one.The process will be ended when a qualified hash is found.
        /// </summary>
        /// <param name="difficulty"> Requirement </param>
        public void Mine(int difficulty)
        {
            var leadingZeros = new string('0', difficulty);
            while (this.Hash == null || this.Hash.Substring(0, difficulty) != leadingZeros)
            {
                this.Nonce++;
                this.Hash = this.CalculateHash();
            }
        }
    }
}
