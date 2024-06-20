using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVX.Serialization
{
    public struct STRONGS
    {
        public ulong elements { get; private set; }
        public ushort this[int idx]
        {
            get
            {
                var shift = (idx - 1) * 16;
                switch (idx)
                {
                    case 3:
                    case 2:
                    case 1:
                    case 0: return (ushort)(elements >> shift & 0xFFFF);
                }
                return 0;
            }
            internal set
            {
                switch (idx)
                {
                    case 3:
                    case 2:
                    case 1:
                    case 0: break;
                    default: return; // silent errors
                }
                ulong others = 0x0000FFFFFFFFFFFF;
                ulong shifted = (ulong)(value << 3 * 16);
                for (int segment = 0; segment < idx; segment++)
                {
                    shifted >>= 16;
                    others >>= 16;
                    others |= 0xFFFF000000000000;
                }
                elements = shifted | others;
            }
        }
    }

}
