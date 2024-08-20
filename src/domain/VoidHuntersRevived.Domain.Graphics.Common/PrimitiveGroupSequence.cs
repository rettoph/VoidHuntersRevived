using VoidHuntersRevived.Domain.Graphics.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Common
{
    public struct PrimitiveGroupSequence
    {
        public readonly PrimitiveGroupEnum Group;
        public int Sequence;

        public PrimitiveGroupSequence(PrimitiveGroupEnum group, int sequence)
        {
            Group = group;
            Sequence = sequence;
        }
    }
}
