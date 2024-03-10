using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Ships.Components
{
    public struct Tractorable : IEntityComponent
    {
        public EntityId TractorBeamEmitter { get; init; }
    }
}
