using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Core.Utilities;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public sealed class HardSpawnEntity : IEventData
    {
        public bool IsPrivate => true;
        public bool IsPredictable => true;

        public required VhId VhId { get; init; }
        public required Id<ITeam> TeamId { get; init; }
        public required IEntityType Type { get; init; }
        public required EntityInitializerDelegate? Initializer { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<SpawnEntity, VhId, VhId, Id<ITeam>, Id<IEntityType>>.Instance.Calculate(in source, this.VhId, this.TeamId, this.Type.Id);
        }
    }
}
