using Svelto.ECS;
using System.Diagnostics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Ships.Services;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Exceptions;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Game.Ships.Events;

namespace VoidHuntersRevived.Game.Ships.Services
{
    public partial class TractorBeamEmitterService : ITractorBeamEmitterService,
        IEventEngine<TractorBeamEmitter_Select>,
        IEventEngine<TractorBeamEmitter_Deselect>
    {
        public void Select(EntityId tractorBeamEmitterId, EntityId nodeId)
        {
            if(!_entities.IsSpawned(nodeId))
            {
                _logger.Warning("{ClassName}::{MethodName} - Entity {EntityVhId} does not exist", nameof(TractorBeamEmitterService), nameof(Select), nodeId.VhId.Value);
                return;
            }

            this.Simulation.Publish(
                sender: NameSpace<TractorBeamEmitterService>.Instance,
                data: new TractorBeamEmitter_Select()
                {
                    TractorBeamEmitterVhId = tractorBeamEmitterId.VhId,
                    TargetData = _entities.Serialize(nodeId)
                });

            ref Node node = ref _entities.QueryById<Node>(nodeId);
            if(_nodes.IsHead(in node))
            {
                _entities.Despawn(node.TreeId);
            }
            else
            {
                _entities.Despawn(nodeId);
            }
        }

        public void Deselect(EntityId tractorBeamEmitterId)
        {
            _entities.Flush();
            ref var filter = ref this.GetTractorableFilter(tractorBeamEmitterId);

            foreach (var (indices, groupId) in filter)
            {
                var (entityIds, trees, locations, _) = _entities.QueryEntities<EntityId, Tree, Location>(groupId);

                for (int i = 0; i < indices.count; i++)
                {
                    uint index = indices[i];
                    Tree tree = trees[index];

                    this.Simulation.Publish(
                        sender: NameSpace<TractorBeamEmitterService>.Instance,
                        data: new TractorBeamEmitter_Deselect()
                        {
                            TractorBeamEmitterVhId = tractorBeamEmitterId.VhId,
                            TargetData = _entities.Serialize(tree.HeadId),
                            Location = locations[index]
                        });

                    _entities.Despawn(entityIds[index]);
                }
            }
        }

        void IEventEngine<TractorBeamEmitter_Select>.Process(VhId eventId, TractorBeamEmitter_Select data)
        {
            try
            {
                EntityId cloneId = _trees.Spawn(
                    vhid: eventId.Create(1),
                    tree: EntityTypes.Chain,
                    nodes: data.TargetData,
                    initializer: (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                    {
                        if (!entities.TryGetId(data.TractorBeamEmitterVhId, out EntityId tractorBeamEmitterId))
                        {
                            throw new ArgumentException($"Unable to locate {nameof(TractorBeamEmitter)} {data.TractorBeamEmitterVhId.Value}");
                        }

                        initializer.Init<Tractorable>(new Tractorable()
                        {
                            TractorBeamEmitter = tractorBeamEmitterId
                        });
                    });
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "{ClassName}::{MethodName}<{GenericTypeName}> - Exception thrown", nameof(TractorBeamEmitterService), nameof(Process), nameof(TractorBeamEmitter_Select));
                throw new SimulationOutOfSyncException(ex.Message, ex);
            }
        }

        void IEventEngine<TractorBeamEmitter_Deselect>.Process(VhId eventId, TractorBeamEmitter_Deselect data)
        {
            try
            {
                EntityId cloneId = _trees.Spawn(
                    vhid: eventId.Create(1),
                    tree: EntityTypes.Chain,
                    nodes: data.TargetData,
                    initializer: (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                    {
                        initializer.Init<Location>(data.Location);
                        initializer.Init<Tractorable>(new Tractorable()
                        {
                            TractorBeamEmitter = default
                        });
                    });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{ClassName}::{MethodName}<{GenericTypeName}> - Exception thrown", nameof(TractorBeamEmitterService), nameof(Process), nameof(TractorBeamEmitter_Select));
                throw new SimulationOutOfSyncException(ex.Message, ex);
            }
        }
    }
}
