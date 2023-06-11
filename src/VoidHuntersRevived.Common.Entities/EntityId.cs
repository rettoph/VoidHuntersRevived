using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities
{
    [StructLayout(LayoutKind.Explicit)]
    public struct EntityId
    {
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

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
