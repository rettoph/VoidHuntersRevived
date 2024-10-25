using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Entities.Common.Events
{
    public class SpawnEntity : IEventData
    {
        public required bool IsPrivate { get; init; }

        public bool IsPredictable => true;

        public required VhId VhId { get; init; }
        public required Key<IEntityTemplate> TemplateKey { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<SpawnEntity, VhId, VhId, VhId>.Instance.Calculate(in source, this.VhId, this.TemplateKey.Id);
        }
    }

    public class SpawnEntity<TInitializer> : SpawnEntity
    {
        public required TInitializer Initializer { get; init; }
    }
}
