﻿using Guppy.Attributes;
using Guppy.Resources.Providers;
using Serilog;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    [AutoLoad]
    internal sealed class RigidEngine : BasicEngine,
        IOnSpawnEngine<Rigid>,
        IReactOnRemoveEx<Rigid>
    {
        private readonly ISpace _space;
        private readonly IEntityService _entities;
        private readonly ILogger _logger;

        public RigidEngine(ISpace space, IEntityService entities, ILogger logger)
        {
            _space = space;
            _entities = entities;
            _logger = logger;
        }

        public void OnSpawn(EntityId id, ref Rigid rigid, in GroupIndex groupIndex)
        {
            Node node = _entities.QueryByGroupIndex<Node>(in groupIndex);

            IBody body = _space.GetOrCreateBody(node.TreeId);

            body.Create(id.VhId, rigid.Shapes[0], node.LocalLocation.Transformation);
        }

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<Rigid> entities, ExclusiveGroupStruct groupID)
        {
            var (rigids, _, _) = entities;
            var (ids, nodes, _) = _entities.QueryEntities<EntityId, Node>(groupID);

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                EntityId id = ids[index];
                Node node = nodes[index];

                if (_space.TryGetBody(node.TreeId, out IBody? body))
                {
                    body.Destroy(id.VhId);
                }
            }
        }
    }
}
