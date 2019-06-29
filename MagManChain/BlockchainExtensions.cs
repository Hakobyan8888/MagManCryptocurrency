using System.Collections.Generic;

namespace MagMan
{
    public static class BlockchainExtensions
    {
        /// <summary>
        /// Check the validity of the chain, to be sure that the chain was not hacked, was not tampered. 
        /// </summary>
        /// <returns> False if the block was modified or tampered, otherwise true.</returns>
        public static bool IsValid(this Blockchain blockChain)
        {
            for (int i = 1; i < blockChain.Chain.Count; i++)
            {
                Block currentBlock = blockChain.Chain[i];
                Block previousBlock = blockChain.Chain[i - 1];

                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    return false;
                }

                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
