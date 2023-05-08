using Standart.Hash.xxHash;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common.Simulations.Extensions.System;
using VoidHuntersRevived.Common.Simulations.Providers;

namespace VoidHuntersRevived.Common.Simulations
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct ParallelKey
    {
        private const int _bufferSizeInULong = 4;
        private static readonly int _bufferSizeInBytes = sizeof(ulong) * _bufferSizeInULong;
        private static ulong[] _buffer = new ulong[_bufferSizeInULong];

        public static ParallelKey Empty = new ParallelKey(0);

        [FieldOffset(0)]
        private readonly ulong _lower;

        [FieldOffset(64)]
        private readonly ulong _upper;

        [FieldOffset(0)]
        public readonly UInt128 Value;


        internal unsafe ParallelKey(UInt128 value)
        {
            this.Value = value;
        }

        public unsafe ParallelKey Step(ulong count)
        {
            _buffer[0] = _lower;
            _buffer[1] = _upper;
            _buffer[2] = count;
            _buffer[3] = count;

            return Hash();
        }

        public unsafe ParallelKey Merge(ParallelKey noise)
        {
            _buffer[0] = _lower;
            _buffer[1] = _upper;
            _buffer[2] = noise._lower;
            _buffer[3] = noise._upper;

            return Hash();
        }

        private static unsafe ParallelKey Hash()
        {
            fixed (ulong* pBuffer = _buffer)
            {
                Span<byte> dataSpan = new Span<byte>((byte*)pBuffer, _bufferSizeInBytes);
                uint128 hash = xxHash128.ComputeHash(dataSpan, _bufferSizeInBytes);
                UInt128* value = (UInt128*)&hash;

                return new ParallelKey(value[0]);
            }
        }

        public unsafe static ParallelKey NewKey()
        {
            Guid valueGuid = Guid.NewGuid();
            UInt128* value = (UInt128*)&valueGuid;

            return new ParallelKey(value[0]);
        }

        public static ParallelKey operator ++(ParallelKey a)
        {
            return a.Step(1);
        }

        public static ParallelKey operator +(ParallelKey a, ulong b)
        {
            return a.Step(b);
        }
    }
}
