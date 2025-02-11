using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public class SpawnEntity : IStepEvent<SpawnEntity>
    {
        public required bool IsPrivate { get; init; }
        public bool IsPredictable => true;

        public required EntityGlobalId GlobalId { get; init; }
        public required Key<IEntityTemplate> TemplateKey { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<SpawnEntity, VhId, EntityGlobalId, VhId>.Instance.Calculate(in source, this.GlobalId, this.TemplateKey.Id);
        }
    }

    public class SpawnEntity<TInitializer> : IStepEvent<SpawnEntity<TInitializer>>
    {
        public required bool IsPrivate { get; init; }
        public bool IsPredictable => true;

        public required EntityGlobalId GlobalId { get; init; }
        public required Key<IEntityTemplate> TemplateKey { get; init; }
        public required TInitializer Initializer { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<SpawnEntity, VhId, EntityGlobalId, VhId>.Instance.Calculate(in source, this.GlobalId, this.TemplateKey.Id);
        }
    }
}