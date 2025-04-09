using Guppy.Core.Assets.Common;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Components;

namespace VoidHuntersRevived.Domain.Physics.Common.Components
{
    public readonly struct Rigid(Asset<IBodyTemplate> template) : IEntityComponent, ICompositeBelongsTo<Body, Fixture, Rigid>
    {
        public readonly Asset<IBodyTemplate> Template = template;
    }
}