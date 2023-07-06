﻿using Guppy.Attributes;
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
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Common.Components;
using VoidHuntersRevived.Game.Pieces.Components;

namespace VoidHuntersRevived.Game.Pieces.Engines
{
    [AutoLoad]
    internal sealed class BodyEngine : BasicEngine,
        IReactOnAddAndRemoveEx<Body>, IStepEngine<Step>
    {
        private readonly IEntityService _entities;
        private readonly ISpace _space;

        public BodyEngine(IEntityService entities, ISpace space)
        {
            _entities = entities;
            _space = space;
        }

        public string name { get; } = nameof(BodyEngine);

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Body> entities, ExclusiveGroupStruct groupID)
        {
            var (bodies, ids, _) = entities;

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                IdMap bodyId = _entities.GetIdMap(ids[index], groupID);
                bodies[index].Id = bodyId.VhId;

                _space.GetOrCreateBody(bodyId.VhId);
            }
        }

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<Body> entities, ExclusiveGroupStruct groupID)
        {
            var (bodies, ids, _) = entities;

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                _space.RemoveBody(bodies[index].Id);
            }
        }

        public void Step(in Step _param)
        {
            LocalFasterReadOnlyList<ExclusiveGroupStruct> groups = this.entitiesDB.FindGroups<EntityVhId, Body>();
            foreach (var ((vhids, bodies, count), _) in this.entitiesDB.QueryEntities<EntityVhId, Body>(groups))
            {
                for (int i = 0; i < count; i++)
                {
                    IBody bodyInstance = _space.GetBody(vhids[i].Value);
                    ref Body bodyComponent = ref bodies[i];

                    bodyComponent.Position = bodyInstance.Position;
                    bodyComponent.Rotation = bodyInstance.Rotation;
                    // bodyComponent.Transformation = FixMatrix.CreateRotationZ(bodyComponent.Rotation) * FixMatrix.CreateTranslation(bodyComponent.Position.X, bodyComponent.Position.Y, Fix64.Zero);
                }
            }
        }
    }
}
