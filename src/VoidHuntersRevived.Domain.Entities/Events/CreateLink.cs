using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public sealed class CreateLink : IData
    {
        public required ParallelKey Parent { get; init; }
        public required int ParentJointId { get; init; }
        public required ParallelKey Child { get; init; }
        public required int ChildJointId { get; init; }

        public override bool Equals(object? obj)
        {
            return obj is CreateLink link &&
                   EqualityComparer<ParallelKey>.Default.Equals(Parent, link.Parent) &&
                   EqualityComparer<int>.Default.Equals(ParentJointId, link.ParentJointId) &&
                   EqualityComparer<ParallelKey>.Default.Equals(Child, link.Child) &&
                   EqualityComparer<int>.Default.Equals(ChildJointId, link.ChildJointId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Parent, ParentJointId, Child, ChildJointId);
        }
    }
}
