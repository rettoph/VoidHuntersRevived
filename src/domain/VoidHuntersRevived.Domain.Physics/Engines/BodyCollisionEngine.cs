using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Physics.Engines
{
    [AutoLoad]
    public sealed class BodyCollisionEngine : StrategyEngine
    {
        private readonly IEntityQueryService _entityQueryService;
        private readonly ISpace _space;

        public BodyCollisionEngine(
            IEntityQueryService entityQueryService,
            ISpace space)
        {
            _entityQueryService = entityQueryService;
            _space = space;

            _space.OnBodyEnabled += this.HandleBodyEnabled;
        }

        private void HandleBodyEnabled(IBody body)
        {
            ref Collision collision = ref _entityQueryService.QueryById<Collision>(body.Id);
            body.CollisionCategories = collision.Categories;
            body.CollidesWith = collision.CollidesWith;
        }
    }
}
