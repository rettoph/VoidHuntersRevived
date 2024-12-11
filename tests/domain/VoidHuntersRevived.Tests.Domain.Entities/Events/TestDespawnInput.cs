using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Tests.Domain.Entities.Events
{
    internal class TestDepawnInput : IInputData
    {
        public VhId ShipVhId => throw new NotImplementedException();

        public bool IsPredictable => true;

        public required EntityGlobalId EntityGlobalId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TestDepawnInput, VhId, EntityGlobalId>.Instance.Calculate(source, this.EntityGlobalId);
        }
    }
}
