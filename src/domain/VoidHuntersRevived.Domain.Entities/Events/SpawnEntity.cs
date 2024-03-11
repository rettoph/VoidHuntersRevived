using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public sealed class SpawnEntity : IEventData
    {
        public bool IsPredictable => true;

        public required VhId VhId { get; init; }
        public required Id<ITeam> TeamId { get; init; }
        public required IEntityType Type { get; init; }
        public required InstanceEntityInitializerDelegate? Initializer { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<SpawnEntity, VhId, VhId, Id<ITeam>, Id<IEntityType>>.Instance.Calculate(in source, this.VhId, this.TeamId, this.Type.Id);
        }
    }
}
