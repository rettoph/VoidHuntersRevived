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
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Pieces;

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal sealed class TreeEngine : BasicEngine, IReactOnAddEx<Tree>,
        IStepEngine<Step>
    {
        private readonly Map<CombinedFilterID, EGID> _filterEgIds = new Map<CombinedFilterID, EGID>();
        private int _treeFilterId;

        public string name { get; } = nameof(TreeEngine);

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Tree> entities, ExclusiveGroupStruct groupID)
        {
            var (trees, ids, _) = entities;

            for(uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                this.CreateFilter(
                    treeId: this.Simulation.Entities.GetIdMap(ids[index], groupID),
                    headId: this.Simulation.Entities.GetIdMap(trees[index].HeadId));
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

                foreach(var (indices, group) in filter)
                {
                    var (nodes, _) = entitiesDB.QueryEntities<Node>(group);

                    for(int i=0; i<indices.count; i++)
                    {
                        Node node = nodes[i];
                        Console.WriteLine(node.TreeId.Value);
                    }
                }
            }
        }

        private void CreateFilter(in IdMap treeId, in IdMap headId)
        {
            var filterId = new CombinedFilterID(_treeFilterId++, Tree.FilterContextID);
            this.entitiesDB.GetFilters().CreatePersistentFilter<Tree>(filterId);
            _filterEgIds.TryAdd(filterId, treeId.EGID);

            this.AddNodeToTree(treeId, headId);            
        }

        private void AddNodeToTree(in IdMap treeId, in IdMap nodeId)
        {
            var filters = this.entitiesDB.GetFilters();
            var filter = filters.GetPersistentFilter<Tree>(_filterEgIds[treeId.EGID]);

            var nodes = this.entitiesDB.QueryEntitiesAndIndex<Node>(nodeId.EGID, out uint index);
            filter.Add(nodeId.EGID.entityID, nodeId.EGID.groupID, index);

            nodes[index].TreeId = treeId.VhId;
        }
    }
}
