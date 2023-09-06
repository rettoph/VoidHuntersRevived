using Guppy.Attributes;
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
    internal sealed class BodyLocationEngine : BasicEngine,
        IReactOnAddAndRemoveEx<Location>, IStepEngine<Step>
    {
        private readonly IEntityService _entities;
        private readonly ISpace _space;

        public BodyLocationEngine(IEntityService entities, ISpace space)
        {
            _entities = entities;
            _space = space;
        }

        public string name { get; } = nameof(BodyLocationEngine);

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Location> entities, ExclusiveGroupStruct groupID)
        {
            var (locations, _) = entities;
            var (ids, collisions, _) = _entities.QueryEntities<EntityId, Collision>(groupID);

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                IBody body = _space.GetOrCreateBody(ids[index].VhId);
                body.CollisionCategories = collisions[index].Categories;
                body.CollidesWith = collisions[index].CollidesWith;
                body.SetTransform(locations[index].Position, locations[index].Rotation);
            }
        }

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<Location> entities, ExclusiveGroupStruct groupID)
        {
            var (ids, _) = _entities.QueryEntities<EntityId>(groupID);

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                _space.DestroyBody(ids[index].VhId);
            }
        }

        public void Step(in Step _param)
        {
            foreach (var ((ids, locations, awakes, count), _) in _entities.QueryEntities<EntityId, Location, Awake>())
            {
                for (int i = 0; i < count; i++)
                {
                    if(awakes[i] == false)
                    {
                        continue;
                    }

                    IBody body = _space.GetBody(ids[i].VhId);

                    ref Location location = ref locations[i];
                    location.Position = body.Position;
                    location.Rotation = body.Rotation;
                }
            }
        }
    }
}
