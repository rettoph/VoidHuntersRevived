using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Ships.Common.Components
{
    public struct Tractorable : IEntityComponent
    {
        public EntityId TractorBeamEmitter { get; init; }
    }
}
