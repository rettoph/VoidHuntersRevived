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
    internal sealed class BodyEngine : BasicEngine,
        IReactOnAddAndRemoveEx<Location>, IStepEngine<Step>
    {
        private readonly IEntityService _entities;
        private readonly ISpace _space;

        public BodyEngine(IEntityService entities, ISpace space)
        {
            _entities = entities;
            _space = space;
        }

        public string name { get; } = nameof(BodyEngine);

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Location> entities, ExclusiveGroupStruct groupID)
        {
            var (ids, collisions, _) = this.entitiesDB.QueryEntities<EntityId, Collision>(groupID);

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                IBody body = _space.GetOrCreateBody(ids[index].VhId);
                body.CollisionCategories = collisions[index].Categories;
                body.CollidesWith = collisions[index].CollidesWith;
            }
        }

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<Location> entities, ExclusiveGroupStruct groupID)
        {
            var (ids, _) = this.entitiesDB.QueryEntities<EntityId>(groupID);

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                _space.DestroyBody(ids[index].VhId);
            }
        }

        public void Step(in Step _param)
        {
            LocalFasterReadOnlyList<ExclusiveGroupStruct> groups = this.entitiesDB.FindGroups<Location>();
            foreach (var ((ids, bodies, count), _) in this.entitiesDB.QueryEntities<EntityId, Location>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    IBody bodyInstance = _space.GetBody(ids[i].VhId);
                    ref Location bodyComponent = ref bodies[i];

                    bodyComponent.Position = bodyInstance.Position;
                    bodyComponent.Rotation = bodyInstance.Rotation;
                    // bodyComponent.Transformation = FixMatrix.CreateRotationZ(bodyComponent.Rotation) * FixMatrix.CreateTranslation(bodyComponent.Position.X, bodyComponent.Position.Y, Fix64.Zero);
                }
            }
        }
    }
}
