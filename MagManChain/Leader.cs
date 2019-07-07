namespace MagMan
{
    /// <summary>
    /// Info for leader
    /// </summary>
    public class Leader
    {
        public string LeaderAddress { get; set; } = "leader@gmail.com"; // Leader address
        public decimal LeaderAmount { get; set; } = 23000000; // MagMan coins

        /// <summary>
        /// Adds coins
        /// </summary>
        private void AddCoins()
        {
            if (LeaderAmount < 1000000)
            {
                LeaderAmount += 1000000;
            }
        }

    }
}
