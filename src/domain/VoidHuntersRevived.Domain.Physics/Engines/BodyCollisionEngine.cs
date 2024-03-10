using Guppy.Attributes;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Physics.Engines
{
    [AutoLoad]
    internal sealed class BodyCollisionEngine : BasicEngine
    {
        private readonly IEntityService _entities;
        private readonly ISpace _space;

        public BodyCollisionEngine(IEntityService entities, ISpace space)
        {
            _entities = entities;
            _space = space;

            _space.OnBodyEnabled += this.HandleBodyEnabled;
        }

        private void HandleBodyEnabled(IBody body)
        {
            ref Collision collision = ref _entities.QueryById<Collision>(body.Id);
            body.CollisionCategories = collision.Categories;
            body.CollidesWith = collision.CollidesWith;
        }
    }
}
