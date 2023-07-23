using Guppy.Attributes;
using Serilog;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Pieces.Events;

namespace VoidHuntersRevived.Game.Pieces.Engines
{
    [AutoLoad]
    internal sealed class NodeEngine : BasicEngine,
        IReactOnAddEx<Node>,
        IReactOnRemoveEx<Node>
    {
        private readonly IEntityService _entities;
        private readonly IFilterService _filters;
        private readonly ILogger _logger;

        public NodeEngine(IEntityService entities, IFilterService filters, ILogger logger)
        {
            _entities = entities;
            _filters = filters;
            _logger = logger;
        }

        public void Process(VhId eventId, DestroyNode data)
        {
            // this.Simulation.Entities.Destroy(data.NodeId);
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Node> entities, ExclusiveGroupStruct groupID)
        {
            var (nodes, ids, _) = entities;

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                EntityId treeId = nodes[index].TreeId;
                VhId nodeVhId = _entities.GetId(ids[index], groupID).VhId;

                ref var filter = ref _filters.GetFilter<Node>(treeId.VhId);
                filter.Add(ids[index], groupID, index);

                this.Simulation.Publish(NameSpace<NodeEngine>.Instance, new CreateNode()
                {
                    TreeId = treeId.VhId,
                    NodeId = nodeVhId
                });
            }
        }

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<Node> entities, ExclusiveGroupStruct groupID)
        {
            var (nodes, ids, _) = entities;

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                EntityId treeId = nodes[index].TreeId;
                VhId nodeVhId = _entities.GetId(ids[index], groupID).VhId;

                ref var filter = ref _filters.GetFilter<Node>(treeId.VhId);
                filter.Remove(ids[index], groupID);

                this.Simulation.Publish(NameSpace<NodeEngine>.Instance, new DestroyNode()
                {
                    TreeId = treeId.VhId,
                    NodeId = nodeVhId
                });
            }
        }
    }
}
