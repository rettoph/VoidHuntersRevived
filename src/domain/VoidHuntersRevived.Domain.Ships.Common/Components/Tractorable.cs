using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Ships.Common.Components
{
    public readonly struct Tractorable : IEntityComponent
    {
        public EntityLocalId TractorBeamEmitterLocalId { get; init; }
    }
}
