using System.Runtime.InteropServices;
using VoidHuntersRevived.Domain.Common.Providers;

namespace VoidHuntersRevived.Domain.Providers
{
    public class UniqueNumberProvider : IUniqueNumberProvider
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct UniqueNumberValueUnion
        {
            [FieldOffset(0)]
            public uint UInt32;

            [FieldOffset(0)]
            public int Int32;
        }
        private UniqueNumberValueUnion _value;

        public uint GetUInt32()
        {
            return _value.UInt32++;
        }

        public int GetInt32()
        {
            int value = _value.Int32;

            _value.UInt32++;

            return value;
        }
    }
}
