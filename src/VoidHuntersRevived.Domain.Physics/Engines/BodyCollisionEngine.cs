using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common;

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
