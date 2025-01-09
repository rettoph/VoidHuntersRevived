using Guppy.Core.Resources.Common;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Components;

namespace VoidHuntersRevived.Domain.Physics.Common.Components
{
    public readonly struct Rigid(Resource<IBodyTemplate> template) : IEntityComponent, ICompositeBelongsTo<Body, Fixture, Rigid>
    {
        public readonly Resource<IBodyTemplate> Template = template;
    }
}