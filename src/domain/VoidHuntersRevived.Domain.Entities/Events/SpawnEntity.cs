using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Teams;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Entities.Events
{
    public class SpawnEntity : IEventData
    {
        public bool IsPredictable => true;

        public required VhId VhId { get; init; }
        public required Id<ITeam> TeamId { get; init; }
        public required IEntityType Type { get; init; }

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<SpawnEntity, VhId, VhId, Id<ITeam>, Id<IEntityType>>.Instance.Calculate(in source, this.VhId, this.TeamId, this.Type.Id);
        }
    }

    public class SpawnEntity<TInitializer> : SpawnEntity
    {
        public required TInitializer Initializer { get; init; }
    }
}
