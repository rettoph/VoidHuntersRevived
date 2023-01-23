using LiteNetLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.HashFunction;
using System.Data.HashFunction.xxHash;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Simulations
{
    public readonly struct ParallelKey
    {
        private static readonly IxxHash xxHash = xxHashFactory.Instance.Create(new xxHashConfig()
        {
            HashSizeInBits = 32,
            Seed = 1337
        });

        public readonly int Hash;

        internal ParallelKey(int hash)
        {
            this.Hash = hash;
        }

        public ParallelKey Create(ParallelType type)
        {
            return ParallelKey.From(type, 0, this);
        }

        public ParallelKey Create(ParallelType type, int noise)
        {
            return ParallelKey.From(type, noise, this);
        }

        public static ParallelKey From(ParallelType type, int noise)
        {
            return ParallelKey.From(type, noise, default);
        }

        private static ParallelKey From(ParallelType type, int noise, ParallelKey parent)
        {
            int[] int_data = new int[]
            {
                type.Value,
                noise,
                parent.Hash
            };

            byte[] byte_data = new byte[int_data.Length * sizeof(int)];
            Buffer.BlockCopy(int_data, 0, byte_data, 0, byte_data.Length);
            IHashValue computed = xxHash.ComputeHash(byte_data);
            int hash = BitConverter.ToInt32(computed.Hash);

            return new ParallelKey(hash);
        }
    }
}
