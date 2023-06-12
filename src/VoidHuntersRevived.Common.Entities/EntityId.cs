using Standart.Hash.xxHash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities
{
    [StructLayout(LayoutKind.Explicit)]
    public struct EntityId
    {
        private const int _bufferSizeInULong = 4;
        private static readonly int _bufferSizeInBytes = sizeof(ulong) * _bufferSizeInULong;
        private static ulong[] _buffer = new ulong[_bufferSizeInULong];

        public static EntityId Empty = new EntityId(0);

        [FieldOffset(0)]
        private readonly ulong _lower;

        [FieldOffset(64)]
        private readonly ulong _upper;

        [FieldOffset(0)]
        public readonly UInt128 Value;


        public unsafe EntityId(UInt128 value)
        {
            this.Value = value;
        }

        private static unsafe EntityId Hash()
        {
            fixed (ulong* pBuffer = _buffer)
            {
                Span<byte> dataSpan = new Span<byte>((byte*)pBuffer, _bufferSizeInBytes);
                uint128 hash = xxHash128.ComputeHash(dataSpan, _bufferSizeInBytes);
                UInt128* value = (UInt128*)&hash;

                return new EntityId(value[0]);
            }
        }

        public static unsafe EntityId From(string input)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(input);

            uint128 hash = xxHash128.ComputeHash(bytes, bytes.Length);
            UInt128* value = (UInt128*)&hash;

            return new EntityId(value[0]);
        }

        public static EntityId From<T>()
        {
            return EntityId.From(typeof(T).AssemblyQualifiedName ?? throw new InvalidOperationException());
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
