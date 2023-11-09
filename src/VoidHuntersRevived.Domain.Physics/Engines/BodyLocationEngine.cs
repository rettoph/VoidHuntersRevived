using Guppy.Attributes;
using Guppy.Common.Attributes;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Domain.Physics.Engines
{
    [AutoLoad]
    internal sealed class BodyLocationEngine : BasicEngine, IStepEngine<Step>
    {
        private readonly IEntityService _entities;
        private readonly ISpace _space;

        public BodyLocationEngine(IEntityService entities, ISpace space)
        {
            _entities = entities;
            _space = space;

            _space.OnBodyEnabled += this.HandleBodyEnabled;
        }

        public string name { get; } = nameof(BodyLocationEngine);

        public void Step(in Step _param)
        {
            foreach (var ((ids, locations, enableds, awakes, count), _) in _entities.QueryEntities<EntityId, Location, Enabled, Awake>())
            {
                for (int i = 0; i < count; i++)
                {
                    if(enableds[i] == false || awakes[i] == false)
                    {
                        continue;
                    }

                    IBody body = _space.GetBody(ids[i]);

                    ref Location location = ref locations[i];
                    location.Position = body.Position;
                    location.Rotation = body.Rotation;
                }
            }
        }

        private void HandleBodyEnabled(IBody body)
        {
            ref Collision collision = ref _entities.QueryById<Collision>(body.Id);
            body.CollisionCategories = collision.Categories;
            body.CollidesWith = collision.CollidesWith;
        }
    }
}
