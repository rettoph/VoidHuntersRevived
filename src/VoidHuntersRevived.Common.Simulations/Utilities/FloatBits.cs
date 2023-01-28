using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Utilities
{
    [StructLayout(LayoutKind.Explicit)]
    public struct FloatBits
    {
        private static readonly BitVector32.Section FractionSectionOne = BitVector32.CreateSection(1 << 14);
        private static readonly BitVector32.Section FractionSectionTwo = BitVector32.CreateSection(1 << 7, FractionSectionOne);
        public static readonly int[] FractionBitMasks;
        public static readonly BitVector32.Section ExponentSection = BitVector32.CreateSection(1 << 7, FractionSectionTwo);
        public const int SignMask = 1 << 31;
        public const int ExponentBias = 127;

        static FloatBits()
        {
            var FractionSectionOne = BitVector32.CreateSection(1 << 14);
            var FractionSectionTwo = BitVector32.CreateSection(1 << 7, FractionSectionOne);
            ExponentSection = BitVector32.CreateSection(1 << 7, FractionSectionTwo);

            FractionBitMasks = new int[23];
            for(int i=0; i<23; i++)
            {
                FractionBitMasks[i] = 1 << i;
            }
        }

        [FieldOffset(0)]
        private BitVector32 _data;

        [FieldOffset(0)]
        private float _value;

        public float Value
        {
            get => _value;
            set => _value = value;
        }

        public BitVector32 Data
        {
            get => _data;
            set => _data = value;
        }

        public int Exponent => _data[ExponentSection] - ExponentBias;

        /// <summary>
        /// One bit will be trimmed for every N difference
        /// between the current exponent and the given
        /// <paramref name="minimumExponent"/>
        /// </summary>
        /// <param name="minimumExponent"></param>
        /// <returns></returns>
        public void Trim(int minimumExponent, out int bits)
        {
            bits = minimumExponent - this.Exponent;

            if(bits <= 0)
            {
                bits = 0;
                return;
            }

            if(bits > 23)
            {
                bits = 23;
            }

            for(int i=0; i<bits; i++)
            {
                _data[FractionBitMasks[i]] = false;
            }
        }
    }
}
