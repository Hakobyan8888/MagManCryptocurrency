using System;
using System.Collections.Generic;

namespace MagMan
{
    public interface IBlock
    {
        int Index { get; set; } // Symbolizes the location of the block inside the blockchain.
        DateTime TimeStamp { get; set; } // Saves the time aspects of when the block was built.
        string Hash { get; set; } // Fixed length number derived from a given message or document.
        string PreviousHash { get; set; } // Hash of a neighbnor node.        
        IList<Transaction> Transactions { get; set; }  // A set of messages or transactions.
        int Nonce { get; set; } // Saves the integer (32 or 64bits) that are utilized in the mining method.

        /// <summary>
        /// Generating hash
        /// </summary>
        /// <returns> Hash </returns>
        string CalculateHash();

        /// <summary>
        /// Find a hash that matches with difficulty
        /// </summary>
        /// <param name="difficulty"> Indicates the number of leading zeros required for a generated hash</param>
        void Mine(int difficulty);
    }
}

