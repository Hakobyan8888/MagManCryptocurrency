using System;
using System.Linq;

namespace MagMan
{
    public static class BlockExtensions
    {
        /// <summary>
        /// Validate that hash 
        /// </summary>
        /// <param name="block"> Current block </param>
        /// <returns></returns>
        public static bool HasValidHash(this IBlock block)
        {
            var current = block.CalculateHash();
            return block.Hash.SequenceEqual(current);
        }

        /// <summary>
        /// Validate the hash of a previous block
        /// </summary>
        /// <param name="block"> Current block </param>
        /// <param name="previousblock"> Previous block </param>
        /// <returns></returns>
        public static bool HasValidPreviousHash(this IBlock block, IBlock previousblock)
        {
            if (previousblock == null)
            {
                throw new ArgumentException("Previous block is null", "previousblock");
            }
            var prev = previousblock.CalculateHash();
            return previousblock.HasValidHash() && block.PreviousHash.SequenceEqual(prev);
        }
    }
}
