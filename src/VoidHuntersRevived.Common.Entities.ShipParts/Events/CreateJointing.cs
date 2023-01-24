using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Events
{
    public sealed class CreateJointing : IData
    {
        public required ParallelKey Parent { get; init; }
        public required int ParentJointId { get; init; }
        public required ParallelKey Joint { get; init; }
        public required int JointId { get; init; }

        public override bool Equals(object? obj)
        {
            return obj is CreateJointing link &&
                   EqualityComparer<ParallelKey>.Default.Equals(Parent, link.Parent) &&
                   EqualityComparer<int>.Default.Equals(ParentJointId, link.ParentJointId) &&
                   EqualityComparer<ParallelKey>.Default.Equals(Joint, link.Joint) &&
                   EqualityComparer<int>.Default.Equals(JointId, link.JointId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Parent, ParentJointId, Joint, JointId);
        }
    }
}
