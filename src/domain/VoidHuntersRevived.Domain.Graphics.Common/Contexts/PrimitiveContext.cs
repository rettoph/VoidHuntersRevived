using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Graphics.Common.Contexts
{
    public readonly struct PrimitiveContext(Key<IPrimitive> type, int sequence)
    {
        public readonly Key<IPrimitive> Type = type;
        public readonly int Sequence = sequence;
    }
}
