using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Tests.Domain.Entities.Events
{
    internal class TestDepawnInput : IInputData
    {
        public VhId ShipVhId => throw new NotImplementedException();

        public bool IsPredictable => true;

        public required VhId EntityId { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TestDepawnInput, VhId, VhId>.Instance.Calculate(source, this.EntityId);
        }
    }
}
