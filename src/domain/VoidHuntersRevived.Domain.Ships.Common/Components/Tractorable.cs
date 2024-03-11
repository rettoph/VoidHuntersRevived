using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Ships.Common.Components
{
    public struct Tractorable : IEntityComponent
    {
        public EntityId TractorBeamEmitter { get; init; }
    }
}
