using System;
using System.Collections.Generic;
using System.Text;

namespace MagMan
{
    public class Leader
    {
        public string LeaderAddress { get { return LeaderAddress; }
            private set { LeaderAddress = "leader@gmail.com"; }
        }

        public decimal LeaderAmount { get { return LeaderAmount; } set { LeaderAmount = 23000000; } }

        public void AddCoins()
        {
            if (LeaderAmount < 1000000)
            {
                LeaderAmount += 1000000;
            }
        }

    }
}
