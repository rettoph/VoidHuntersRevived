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

        public required EntityGlobalId EntityGlobalId { get; init; }
        public required Key<IEntityTemplate> EntityTemplateKey { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<TestSpawnInput, VhId, EntityGlobalId, VhId>.Instance.Calculate(source, this.EntityGlobalId, this.EntityTemplateKey.Id);
        }
    }
}
