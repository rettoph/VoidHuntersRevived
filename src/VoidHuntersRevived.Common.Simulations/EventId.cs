using Standart.Hash.xxHash;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using VoidHuntersRevived.Common.ECS;

namespace VoidHuntersRevived.Common.Simulations
{
    [StructLayout(LayoutKind.Explicit)]
    public struct EventId
    {
        private const int _bufferSizeInULong = 4;
        private static readonly int _bufferSizeInBytes = sizeof(ulong) * _bufferSizeInULong;
        private static ulong[] _buffer = new ulong[_bufferSizeInULong];

        public static EventId Empty = new EventId(0);

        [FieldOffset(0)]
        private readonly ulong _lower;

        [FieldOffset(64)]
        private readonly ulong _upper;

        [FieldOffset(0)]
        public readonly UInt128 Value;


        public unsafe EventId(UInt128 value)
        {
            this.Value = value;
        }

        public EntityId EntityId()
        {
            return Unsafe.As<EventId, EntityId>(ref this);
        }

        public EntityId EntityId(ulong count)
        {
            return this.Step(count).EntityId();
        }

        public unsafe EventId Step(ulong count)
        {
            _buffer[0] = _lower;
            _buffer[1] = _upper;
            _buffer[2] = count;
            _buffer[3] = count;

            return Hash();
        }

        public unsafe EventId Merge(EventId noise)
        {
            _buffer[0] = _lower;
            _buffer[1] = _upper;
            _buffer[2] = noise._lower;
            _buffer[3] = noise._upper;

            return Hash();
        }

        private static unsafe EventId Hash()
        {
            fixed (ulong* pBuffer = _buffer)
            {
                Span<byte> dataSpan = new Span<byte>((byte*)pBuffer, _bufferSizeInBytes);
                uint128 hash = xxHash128.ComputeHash(dataSpan, _bufferSizeInBytes);
                UInt128* value = (UInt128*)&hash;

                return new EventId(value[0]);
            }
        }

        public unsafe static EventId NewId()
        {
            Guid valueGuid = Guid.NewGuid();
            UInt128* value = (UInt128*)&valueGuid;

            return new EventId(value[0]);
        }

        public static unsafe EventId From(string input)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(input);

            uint128 hash = xxHash128.ComputeHash(bytes, bytes.Length);
            UInt128* value = (UInt128*)&hash;

            return new EventId(value[0]);
        }

        public static EventId From<T>()
        {
            return EventId.From(typeof(T).AssemblyQualifiedName ?? throw new InvalidOperationException());
        }

        public static EventId operator ++(EventId a)
        {
            return a.Step(1);
        }

        public static EventId operator +(EventId a, ulong b)
        {
            return a.Step(b);
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
