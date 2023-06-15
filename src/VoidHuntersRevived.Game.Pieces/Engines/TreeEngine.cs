using Guppy.Attributes;
using Guppy.Common.Collections;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Pieces;
using VoidHuntersRevived.Game.Pieces.Components;
using VoidHuntersRevived.Game.Pieces.Events;

namespace VoidHuntersRevived.Game.Pieces.Engines
{
    [AutoLoad]
    internal sealed class TreeEngine : BasicEngine, IReactOnAddEx<Tree>,
        IStepEngine<Step>
    {
        private static readonly VhId CreateFilterEvent = VoidHuntersRevivedGame.NameSpace.Create(nameof(CreateFilterEvent));

        private readonly Map<CombinedFilterID, EGID> _filterEgIds = new Map<CombinedFilterID, EGID>();
        private int _treeFilterId;

        public string name { get; } = nameof(TreeEngine);

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Tree> entities, ExclusiveGroupStruct groupID)
        {
            var (trees, ids, _) = entities;

            for(uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                IdMap treeId = this.Simulation.Entities.GetIdMap(ids[index], groupID);
                IdMap headId = this.Simulation.Entities.GetIdMap(trees[index].HeadId);

                var filterId = new CombinedFilterID(_treeFilterId++, Tree.FilterContextID);
                this.entitiesDB.GetFilters().CreatePersistentFilter<Tree>(filterId);
                _filterEgIds.TryAdd(filterId, treeId.EGID);

                VhId id = CreateFilterEvent.Create(treeId.VhId);
                this.AddNodeToTree(in id, in treeId, in headId);
            }
        }

        public void Step(in Step _param)
        {
            if(!this.entitiesDB.GetFilters().TryGetPersistentFilters<Tree>(Tree.FilterContextID, out var filters))
            {
                return;
            }

            foreach(ref EntityFilterCollection filter in filters)
            {
                EGID treeId = _filterEgIds[filter.combinedFilterID];
                Tree tree = this.entitiesDB.QueryMappedEntities<Tree>(treeId.groupID).Entity(treeId.entityID);

                foreach(var (indices, group) in filter)
                {
                    var (nodes, _) = entitiesDB.QueryEntities<Node>(group);

                    for(int i=0; i<indices.count; i++)
                    {
                        Node node = nodes[indices[i]];
                    }
                }
            }
        }

        private void AddNodeToTree(in VhId id, in IdMap treeId, in IdMap nodeId)
        {
            var filters = this.entitiesDB.GetFilters();
            var filter = filters.GetPersistentFilter<Tree>(_filterEgIds[treeId.EGID]);

            var nodes = this.entitiesDB.QueryEntitiesAndIndex<Node>(nodeId.EGID, out uint index);
            filter.Add(nodeId.EGID.entityID, nodeId.EGID.groupID, index);

            nodes[index].TreeId = treeId.VhId;

            this.Simulation.Publish(id.Create(1), new AddedNode()
            {
                NodeId = nodeId.VhId,
                TreeId = treeId.VhId
            });
        }
    }
}
