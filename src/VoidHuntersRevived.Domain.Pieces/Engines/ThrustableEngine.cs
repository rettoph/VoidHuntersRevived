using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Events;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    [AutoLoad]
    internal class ThrustableEngine : BasicEngine,
        IReactOnAddEx<Thrustable>,
        IEventEngine<Tree_Clean>
    {
        private readonly IEntityService _entities;

        public ThrustableEngine(IEntityService entities)
        {
            _entities = entities;
        }

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Thrustable> entities, ExclusiveGroupStruct groupID)
        {
            var (_, ids, _) = entities;
            var (nodes, _) = _entities.QueryEntities<Node>(groupID);

            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                Node node = nodes[index];

                if (_entities.HasAny<Helm>(node.TreeId.EGID.groupID) == false)
                {
                    continue;
                }

                ref var filter = ref _entities.GetFilter<Thrustable>(node.TreeId, Helm.ThrustableFilterContextId);
                filter.Add(ids[index], groupID, index);
            }
        }

        public void Process(VhId eventId, Tree_Clean data)
        {
            EntityId treeId = _entities.GetId(data.TreeId);
            ref var filter = ref _entities.GetFilter<Thrustable>(treeId, Helm.ThrustableFilterContextId);
            var count = filter.ComputeFinalCount();
        }
    }
}
