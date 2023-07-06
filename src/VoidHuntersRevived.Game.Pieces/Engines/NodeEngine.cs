using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
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

        public NodeEngine(IEntityService entities, IFilterService filters)
        {
            _entities = entities;
            _filters = filters;
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
                VhId treeId = nodes[index].TreeId;
                VhId nodeId = _entities.GetIdMap(ids[index], groupID).VhId;

                Debug.Assert(treeId.Value != VhId.Empty.Value);

                ref var filter = ref _filters.GetFilter<Node>(treeId);
                filter.Add(ids[index], groupID, index);

                this.Simulation.Publish(CreateNode.NameSpace.Create(nodeId), new CreateNode()
                {
                    TreeId = treeId,
                    NodeId = nodeId
                });
            }
        }

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<Node> entities, ExclusiveGroupStruct groupID)
        {
            var (nodes, ids, _) = entities;

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                VhId treeId = nodes[index].TreeId;
                VhId nodeId = _entities.GetIdMap(ids[index], groupID).VhId;

                ref var filter = ref _filters.GetFilter<Node>(treeId);
                filter.Remove(ids[index], groupID);

                this.Simulation.Publish(DestroyNode.NameSpace.Create(nodeId), new DestroyNode()
                {
                    TreeId = treeId,
                    NodeId = nodeId
                });
            }
        }
    }
}
