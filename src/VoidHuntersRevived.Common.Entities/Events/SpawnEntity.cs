using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Common.Entities.Events
{
    public sealed class SpawnEntity : IEventData
    {
        public required VhId VhId { get; init; }
        public required IEntityType Type { get; init; }
        public required EntityInitializerDelegate? Initializer { get; init; }
        public bool Configure { get; init; } = true;

        public VhId CalculateHash(in VhId source)
        {
            return HashBuilder<SpawnEntity, VhId, VhId, VhId, bool>.Instance.Calculate(in source, this.VhId, this.Type.Id, this.Configure);
        }
    }
}
