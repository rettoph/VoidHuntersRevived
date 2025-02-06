using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Physics.Systems
{
    public sealed class BodyCollisionSystem : StrategySystem
    {
        private readonly IEntityQueryService _entityQueryService;
        private readonly ISpace _space;

        public BodyCollisionSystem(
            IEntityQueryService entityQueryService,
            ISpace space)
        {
            this._entityQueryService = entityQueryService;
            this._space = space;

            this._space.OnBodyEnabled += this.HandleBodyEnabled;
        }

        private void HandleBodyEnabled(IBody body)
        {
            ref Collision collision = ref this._entityQueryService.QueryByLocalId<Collision>(body.EntityLocalId);
            body.CollisionCategories = collision.Categories;
            body.CollidesWith = collision.CollidesWith;
        }
    }
}