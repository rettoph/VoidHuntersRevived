using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Tests.Domain.Entities.Events
{
    internal class TestSpawnInput : IInputData
    {
        public VhId ShipVhId => throw new NotImplementedException();

        public bool IsPredictable => true;

        public required VhId EntityId { get; init; }
        public required Key<IEntityTemplate> EntityTemplateKey { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TestSpawnInput, VhId, VhId, VhId>.Instance.Calculate(source, this.EntityId, this.EntityTemplateKey.Id);
        }
    }
}
