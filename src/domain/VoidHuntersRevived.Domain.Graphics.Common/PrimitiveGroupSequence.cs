using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Common
{
    public struct PrimitiveGroupSequence(PrimitiveGroupEnum group, int sequence)
    {
        public readonly PrimitiveGroupEnum Group = group;
        public int Sequence = sequence;
    }
}
