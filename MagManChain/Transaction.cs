namespace MagMan
{
    /// <summary>
    /// The class contains information about money transaction between two adresses
    /// </summary>
    public class Transaction
    {
        public string FromAddress { get; set; } // The adress of person who transfers  money  
        public string ToAddress { get; set; } // The adress of person who recieves money 
        public decimal Amount { get; set; } // The amount of money that is transferrred

        /// <summary>
        /// Initializing
        /// </summary>
        public Transaction(string fromAddress, string toAddress, decimal amount)
        {
            FromAddress = fromAddress;
            ToAddress = toAddress;
            Amount = amount;
        }

        
        public Transaction()
        {

        }
    }
}
