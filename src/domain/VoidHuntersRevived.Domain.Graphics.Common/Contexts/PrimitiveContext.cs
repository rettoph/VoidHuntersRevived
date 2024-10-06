using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Common.Contexts
{
    public struct PrimitiveContextTwo
    {
        public PrimitiveSequenceGroupEnum SequenceGroup { get; set; }
        public int Sequence { get; set; }
    }

    public readonly struct PrimitiveContext(Key<IPrimitive> type, int sequence)
    {
        public readonly Key<IPrimitive> Type = type;
        public readonly int Sequence = sequence;
    }
}
