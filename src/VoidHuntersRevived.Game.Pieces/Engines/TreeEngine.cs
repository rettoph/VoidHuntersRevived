﻿using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Collections;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ECS.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Common.Components;

namespace VoidHuntersRevived.Game.Pieces.Engines
{
    [AutoLoad]
    internal sealed class TreeEngine : BasicEngine,
        ISerializationEngine<Tree>,
        IStepEngine<Step>
    {
        public string name { get; } = nameof(TreeEngine);

        private HashSet<EGID> _removedNodes = new HashSet<EGID>();
        private readonly IEntitySerializationService _serialization;
        private readonly IFilterService _filters;

        public TreeEngine(IEntitySerializationService serialization, IFilterService filters)
        {
            _serialization = serialization;
            _filters = filters;
        }

        // public void Add((uint start, uint end) rangeOfEntities, in EntityCollection<Tree> entities, ExclusiveGroupStruct groupID)
        // 
        // {
        //     var (trees, ids, _) = entities;
        // 
        //     for(uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
        //     {
        //         IdMap treeId = this.Simulation.Entities.GetIdMap(ids[index], groupID);
        //         IdMap headId = this.Simulation.Entities.GetIdMap(trees[index].HeadId);
        // 
        //         ref var filter = ref this.entitiesDB.GetFilters().GetOrCreatePersistentFilter<Node>(_nodesFilterId++, Tree.FilterContextID);
        //         trees[index].NodesFilterId = filter.combinedFilterID;
        // 
        //         this.Simulation.Publish(AddNodeToTree.NameSpace.Create(headId.VhId), new AddNodeToTree()
        //         {
        //             TreeId = treeId.VhId,
        //             NodeId = headId.VhId
        //         });
        //     }
        // }
        // 
        // public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<Tree> entities, ExclusiveGroupStruct treeGroupId)
        // {
        //     var (trees, _, _) = entities;
        // 
        //     for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
        //     {
        //         VhId headId = trees[index].HeadId;
        //         this.Simulation.Publish(RemoveNodeFromTree.NameSpace.Create(headId), new RemoveNodeFromTree()
        //         {
        //             NodeId = headId
        //         });
        //     }
        // }

        public void Remove((uint start, uint end) rangeOfEntities, in EntityCollection<Tree> entities, ExclusiveGroupStruct treeGroupId)
        {
            var (trees, _, _) = entities;
        
            for (uint index = rangeOfEntities.start; index < rangeOfEntities.end; index++)
            {
                VhId headId = trees[index].HeadId;
        
                this.Simulation.Publish(DestroyEntity.CreateEvent(headId));
            }
        }

        public void Step(in Step _param)
        {
            LocalFasterReadOnlyList<ExclusiveGroupStruct> groups = this.entitiesDB.FindGroups<Tree>();
            foreach (var ((vhids, trees, egids, count), _) in this.entitiesDB.QueryEntities<EntityVhId, Tree>(groups))
            {
                for (uint treeIndex = 0; treeIndex < count; treeIndex++)
                {
                    ref var filter = ref _filters.GetFilter<Node>(vhids[treeIndex].Value);

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

        // public void Process(VhId eventId, AddNodeToTree data)
        // {
        //     IdMap nodeId = this.Simulation.Entities.GetIdMap(data.NodeId);
        //     IdMap treeId = this.Simulation.Entities.GetIdMap(data.TreeId);
        // 
        //     Tree tree = this.entitiesDB.QueryEntity<Tree>(treeId.EGID);
        // 
        //     ref var filter = ref this.entitiesDB.GetFilters().GetPersistentFilter<Node>(tree.NodesFilterId);
        // 
        //     var nodes = this.entitiesDB.QueryEntitiesAndIndex<Node>(nodeId.EGID, out uint index);
        //     filter.Add(nodeId.EGID.entityID, nodeId.EGID.groupID, index);
        // 
        //     nodes[index].TreeId = treeId.VhId;
        // 
        //     this.Simulation.Publish(CreateNode.NameSpace.Create(data.NodeId), new CreateNode()
        //     {
        //         NodeId = data.NodeId
        //     });
        // }
        // 
        // public void Process(VhId eventId, RemoveNodeFromTree data)
        // {
        //     this.Simulation.Publish(DestroyNode.NameSpace.Create(data.NodeId), new DestroyNode()
        //     {
        //         NodeId = data.NodeId
        //     });
        // 
        //     this.Simulation.Publish(DestroyEntity.CreateEvent(data.NodeId));
        // }

        public void Serialize(in Tree tree, EntityWriter writer)
        {
            _serialization.Serialize(tree.HeadId, writer);
        }

        public void Deserialize(in VhId seed, EntityReader reader, ref Tree component)
        {
            _serialization.Deserialize(in seed, reader);
        }

        // private void AddNodeToTree(in VhId eventId, in IdMap treeId, in Tree tree, in IdMap nodeId)
        // {
        //     var filters = this.entitiesDB.GetFilters();
        //     ref var filter = ref this.entitiesDB.GetFilters().GetPersistentFilter<Tree>(tree.NodesFilterId);
        // 
        //     var nodes = this.entitiesDB.QueryEntitiesAndIndex<Node>(nodeId.EGID, out uint index);
        //     filter.Add(nodeId.EGID.entityID, nodeId.EGID.groupID, index);
        // 
        //     nodes[index].TreeId = treeId.VhId;
        // 
        //     this.Simulation.Publish(eventId.Create(1), new AddNodeToTree()
        //     {
        //         NodeId = nodeId.VhId,
        //         TreeId = treeId.VhId
        //     });
        // }
        // 
        // private void RemoveNodeFromTree(in VhId eventId, in IdMap treeId, in Tree tree, in IdMap nodeId)
        // {
        //     var filters = this.entitiesDB.GetFilters();
        //     ref var filter = ref this.entitiesDB.GetFilters().GetPersistentFilter<Tree>(tree.NodesFilterId);
        // 
        //     filter.Remove(nodeId.EGID);
        // 
        //     this.Simulation.Publish(eventId.Create(1), new RemoveNodeFromTree()
        //     {
        //         NodeId = nodeId.VhId,
        //         TreeId = treeId.VhId
        //     });
        // }
    }
}
