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
            return ParallelKey.From(type, this);
        }

        public ParallelKey Create(ParallelType type, params int[] noise)
        {
            return ParallelKey.From(type, this, noise);
        }

        public static ParallelKey From(ParallelType type, params int[] noise)
        {
            return ParallelKey.From(type, default, noise);
        }

        private static ParallelKey From(ParallelType type, ParallelKey parent, params int[] noise)
        {
            int[] int_data = new int[]
            {
                type.Value,
                parent.Hash
            };

            byte[] byte_data = new byte[(int_data.Length + noise.Length) * sizeof(int)];
            Buffer.BlockCopy(int_data, 0, byte_data, 0, int_data.Length * sizeof(int));
            Buffer.BlockCopy(noise, 0, byte_data, int_data.Length * sizeof(int), noise.Length * sizeof(int));
            IHashValue computed = xxHash.ComputeHash(byte_data);
            int hash = BitConverter.ToInt32(computed.Hash);

            return new ParallelKey(hash);
        }
    }
}
