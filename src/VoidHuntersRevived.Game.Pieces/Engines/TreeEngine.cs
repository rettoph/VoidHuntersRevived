using Guppy.Attributes;
using Guppy.Common.Collections;
using Svelto.DataStructures;
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
using VoidHuntersRevived.Common.Physics.Components;
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
        private int _nodesFilterId;

        public string name { get; } = nameof(TreeEngine);

        public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Tree> entities, ExclusiveGroupStruct groupID)
        {
            var (trees, ids, _) = entities;

            for(uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                IdMap treeId = this.Simulation.Entities.GetIdMap(ids[index], groupID);
                IdMap headId = this.Simulation.Entities.GetIdMap(trees[index].HeadId);

                ref var filter = ref this.entitiesDB.GetFilters().GetOrCreatePersistentFilter<Tree>(_nodesFilterId++, Tree.FilterContextID);
                trees[index].NodesFilterId = filter.combinedFilterID;

                VhId id = CreateFilterEvent.Create(treeId.VhId);
                this.AddNodeToTree(in id, in treeId, in trees[index], in headId);
            }
        }
        public void Step(in Step _param)
        {
            LocalFasterReadOnlyList<ExclusiveGroupStruct> groups = this.entitiesDB.FindGroups<Tree>();
            foreach (var ((trees, count), _) in this.entitiesDB.QueryEntities<Tree>(groups))
            {
                for (uint treeIndex = 0; treeIndex < count; treeIndex++)
                {
                    ref var filter = ref this.entitiesDB.GetFilters().GetPersistentFilter<Tree>(trees[treeIndex].NodesFilterId);

                    foreach (var (nodeIndices, group) in filter)
                    {
                        var (nodes, _) = entitiesDB.QueryEntities<Node>(group);

                        for (int i = 0; i < nodeIndices.count; i++)
                        {
                            nodes[nodeIndices[i]].Transformation = trees[treeIndex].Transformation;
                        }
                    }
                }
            }

            // if (!this.entitiesDB.GetFilters().TryGetPersistentFilters<Tree>(Tree.FilterContextID, out var filters))
            // {
            //     return;
            // }
            // 
            // foreach(ref EntityFilterCollection filter in filters)
            // {
            //     EGID treeId = this.Simulation.World.Filters.GetEGID<Tree>(filter.combinedFilterID);
            //     Tree tree = this.entitiesDB.QueryMappedEntities<Tree>(treeId.groupID).Entity(treeId.entityID);
            // 
            //     foreach(var (indices, group) in filter)
            //         var (nodes, _) = entitiesDB.QueryEntities<Node>(group);
            // 
            //         for(int i=0; i<indices.count; i++)
            //         {
            //             Node node = nodes[indices[i]];
            //         }
            //     }
            // }
        }

        private void AddNodeToTree(in VhId id, in IdMap treeId, in Tree tree, in IdMap nodeId)
        {
            var filters = this.entitiesDB.GetFilters();
            ref var filter = ref this.entitiesDB.GetFilters().GetPersistentFilter<Tree>(tree.NodesFilterId);

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
