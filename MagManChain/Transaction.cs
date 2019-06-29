using System;
using System.Collections.Generic;
using System.Text;


namespace MagMan
{
    /// <summary>
    ///The class contains information about money transaction between two adresses
    /// </summary>
    public class Transaction
    {
        public string FromAddress { get; set; } //the adress of person who transfers  money  
        public string ToAddress { get; set; } // the adress of person who recieves money 
        public int Amount { get; set; } //the amount of money that is transferrred

        /// <summary>
        /// Initializing
        /// </summary>
        public Transaction(string fromAddress, string toAddress, int amount)
        {
            FromAddress = fromAddress;
            ToAddress = toAddress;
            Amount = amount;
        }
    }
}
