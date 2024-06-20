using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVX.Serialization
{
    public struct BCVW
    {
        public BCVW(byte b, byte c, byte v, byte wc)
        {
            this.elements = (UInt32)(b << 24 | c << 16 | v << 8 | wc);
        }
        public UInt32 elements { get; private set; }

        internal byte this[int idx]
        {
            get
            {
                switch (idx)
                {
                    case 0: return this.B;
                    case 1: return this.C;
                    case 2: return this.V;
                    case 3: return this.WC;
                    default: return 0; // silent error (except, obviously bad value)
                }
            }
            set
            {
                UInt32 nibbles = value;

                switch (idx)
                {
                    case 0:
                        elements &= 0x00FFFFFF;
                        elements |= (nibbles << (8 * 3));
                        return;
                    case 1:
                        elements &= 0xFF00FFFF;
                        elements |= (nibbles << (8 * 2));
                        return;
                    case 2:
                        elements &= 0xFFFF00FF;
                        elements |= (nibbles << 8);
                        return;
                    case 3:
                        elements &= 0xFFFFFF00;
                        elements |= nibbles;
                        return;
                    default:
                        elements = 0xFFFFFF00;
                        return; // silent error (except, obviously bad value)
                }
            }
        }
        public byte B
        {
            get => (byte)(this.elements >> (8 * 3));
        }
        public byte C
        {
            get => (byte)((this.elements >> (8 * 2)) & 0xFF);
        }
        public byte V
        {
            get => (byte)((this.elements >> 8) & 0xFF);
        }
        public byte WC
        {
            get => (byte)(this.elements & 0xFF);
        }
        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == typeof(BCVW) && ((BCVW)obj).elements == this.elements;
        }
        public bool StartsWith(byte b, byte c = 0, byte v = 0)
        {
            if (this.B != b)
                return false;
            if (this.C != c || c == 0)
                return (c == 0 && v == 0);
            return (this.V == v || v == 0);
        }
        public static bool operator ==(BCVW bcvw1, BCVW bcvw2)
        {
            return bcvw1.Equals(bcvw2);
        }
        public static bool operator !=(BCVW bcvw1, BCVW bcvw2)
        {
            return !bcvw1.Equals(bcvw2);
        }
        public override int GetHashCode()
        {
            return this.elements.GetHashCode();
        }
        public static bool operator <(BCVW left, BCVW right)
        {
            if (left.elements == right.elements)
                return false;

            UInt32 L_BCV = left.elements & 0xFFFFFF00;
            UInt32 R_BCV = right.elements & 0xFFFFFF00;

            if (L_BCV > R_BCV)
                return false;

            if (L_BCV < R_BCV)
                return true;

            UInt32 L_WC = left.elements & 0xFF;
            UInt32 R_WC = right.elements & 0xFF;

            return (R_WC < L_WC);    // WC is a countdown. Therefore when this condition is true, Left is less than right (positionally)
        }
        public static bool operator >(BCVW left, BCVW right)
        {
            if (left.elements == right.elements)
                return false;

            UInt32 L_BCV = left.elements & 0xFFFFFF00;
            UInt32 R_BCV = right.elements & 0xFFFFFF00;

            if (L_BCV < R_BCV)
                return false;

            if (L_BCV > R_BCV)
                return true;

            UInt32 L_WC = left.elements & 0xFF;
            UInt32 R_WC = right.elements & 0xFF;

            return (R_WC > L_WC);    // WC is a countdown. Therefore when this condition is true, Left is greater than right (positionally)
        }
        public static bool operator <=(BCVW left, BCVW right)
        {
            if (left.elements == right.elements)
                return true;

            UInt32 L_BCV = left.elements & 0xFFFFFF00;
            UInt32 R_BCV = right.elements & 0xFFFFFF00;

            if (L_BCV > R_BCV)
                return false;

            if (L_BCV < R_BCV)
                return true;

            UInt32 L_WC = left.elements & 0xFF;
            UInt32 R_WC = right.elements & 0xFF;

            return (R_WC < L_WC);    // WC is a countdown. Therefore when this condition is true, Left is less than right (positionally)
        }
        public static bool operator >=(BCVW left, BCVW right)
        {
            if (left.elements == right.elements)
                return true;

            UInt32 L_BCV = left.elements & 0xFFFFFF00;
            UInt32 R_BCV = right.elements & 0xFFFFFF00;

            if (L_BCV < R_BCV)
                return false;

            if (L_BCV > R_BCV)
                return true;

            UInt32 L_WC = left.elements & 0xFF;
            UInt32 R_WC = right.elements & 0xFF;

            return (R_WC > L_WC);    // WC is a countdown. Therefore when this condition is true, Left is greater than right (positionally)
        }
        public static (int distance, bool valid) operator -(BCVW left, BCVW right)
        {
            if (left.elements == right.elements)
                return (0, true);

            if (left.B != right.B)
                return (0, false);

            UInt32 L_BCV = left.elements & 0xFFFFFF00;
            UInt32 R_BCV = right.elements & 0xFFFFFF00;

            UInt32 L_WC = left.elements & 0xFF;
            UInt32 R_WC = right.elements & 0xFF;

            if (L_BCV == R_BCV)
            {
                return ((int)L_WC - (int)R_WC, true);
            }
            return (0, false);    // distance can only be calculated with Writ instance
        }
        public bool InRange(byte b, byte c, byte v)
        {
            UInt32 elements = (UInt32)(b << 24 | c << 16 | v << 8);

            return (elements & this.elements) == elements;
        }
        public bool InRange(byte b, byte c)
        {
            UInt32 elements = (UInt32)(b << 24 | c << 16);

            return (elements & this.elements) == elements;
        }
        public bool InRange(byte b)
        {
            return this.B == b;
        }
    }
}
