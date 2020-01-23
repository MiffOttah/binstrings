using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox
{
    /// <summary>
    /// Allows searching BinStrings for a given substring (needle) using the Boyer-Moore search algorithm.
    /// </summary>
    public class BinBoyerMoore
    {
        // Code adapted from the Wikipedias

        private const int ALPHABET_SIZE = 0x100;

        /// <summary>
        /// The BinString that this instance of BinBoyerMoore is searching for.
        /// </summary>
        public BinString Needle { get; protected set; }

        private readonly int[] _CharTable;
        private readonly int[] _OffsetTable;

        /// <summary>
        /// Creates a new instance of the binary Boyer-Moore search.
        /// </summary>
        /// <param name="needle">The substring being searched for.</param>
        /// <exception cref="ArgumentNullException"><paramref name="needle"/> is null.</exception>
        public BinBoyerMoore(BinString needle)
        {
            Needle = needle ?? throw new ArgumentNullException(nameof(needle));
            _CharTable = _MakeCharTable();
            _OffsetTable = _MakeOffsetTable();
        }

        /// <summary>
        /// Searched the provided BinString for the first occurrence of the needle. 
        /// </summary>
        /// <param name="haystack">The BinString to search for the needle.</param>
        /// <returns>If the needle is found in the haystack, returns the (zero-based) index in the haystack where the needle was found. Otherwise, returns -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="haystack"/> is null.</exception>
        public int FindNeedleIn(BinString haystack)
        {
            if (haystack is null) throw new ArgumentNullException(nameof(haystack));
            if (Needle.Length == 0) return 0;
            
            int i = Needle.Length - 1;
            while (i < haystack.Length)
            {
                int j = Needle.Length - 1;
                while (Needle[j] == haystack[i])
                {
                    if (j == 0)
                    {
                        return i;
                    }
                    i--;
                    j--;
                }
                i += Math.Max(_OffsetTable[Needle.Length - (j + 1)], _CharTable[haystack[i]]);
            }
            
            return -1;
        }

        private int[] _MakeCharTable()
        {
            int[] table = new int[ALPHABET_SIZE];
            for (int i = 0; i < table.Length; i++)
            {
                table[i] = Needle.Length;
            }
            for (int i = 0; i < Needle.Length; i++)
            {
                table[Needle[i]] = (Needle.Length - 1) - i;
            }
            return table;
        }

        private int[] _MakeOffsetTable()
        {
            int[] table = new int[Needle.Length];
            int lastPrefixPosition = Needle.Length;
            for (int i = Needle.Length; i > 0; i--)
            {
                if (_IsPrefix(i))
                {
                    lastPrefixPosition = i;
                }
                table[Needle.Length - i] = (lastPrefixPosition - i) + Needle.Length;
            }
            for (int i = 0; i < Needle.Length - 1; i++)
            {
                int slen = _SuffixLength(i);
                table[slen] = (Needle.Length - (i + 1)) + slen;
            }
            return table;
        }

        private bool _IsPrefix(int p)
        {
            int i = p, j = 0;
            while (i < Needle.Length)
            {
                if (Needle[i] != Needle[j]) return false;
                i++; j++;
            }
            return true;
        }

        private int _SuffixLength(int p)
        {
            int l = 0;
            int i = p, j = Needle.Length - 1;
            while (i >= 0 && Needle[i] == Needle[j])
            {
                l++;
                i--;
                j--;
            }
            return l;
        }
    }
}
