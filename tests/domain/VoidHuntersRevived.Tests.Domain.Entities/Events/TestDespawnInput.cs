using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Tests.Domain.Entities.Events
{
    internal class TestDepawnInput : TestInput, IInputData
    {
        public VhId ShipVhId => throw new NotImplementedException();

        public bool IsPredictable => true;

        public required VhId EntityId { get; init; }
        public override required bool DoDiscard { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TestDepawnInput, VhId, VhId, bool>.Instance.Calculate(source, this.EntityId, this.DoDiscard);
        }
    }
}
