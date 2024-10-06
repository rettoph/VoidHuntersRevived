using Svelto.ECS;
using System.Diagnostics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Physics.Common.Extensions.FixedPoint;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Descriptors;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Exceptions;

namespace VoidHuntersRevived.Domain.Ships.Services
{
    public partial class TractorBeamEmitterService : ITractorBeamEmitterService,
        IEventEngine<TractorBeamEmitter_Select>,
        IEventEngine<TractorBeamEmitter_Deselect>
    {
        public void Select(VhId sourceId, EntityId tractorBeamEmitterId, EntityId nodeId)
        {
            if (_entityQueryService.IsSpawned(nodeId, out GroupIndex nodeGroupIndex))
            {
                ref Node node = ref _entityQueryService.QueryByGroupIndex<Node>(nodeGroupIndex);
                if (_entityQueryService.IsSpawned(node.TreeId))
                {
                    _logger.Verbose("{ClassName}::{MethodName} - Selecting {NodeId} with TractorBeamEmitter {TractorBeamEmitterId}", nameof(TractorBeamEmitterService), nameof(Select), nodeId.VhId, tractorBeamEmitterId.VhId);

                    this.Strategy.Publish(
                        sourceId: NameSpace<TractorBeamEmitterService>.Instance.Create(sourceId),
                        data: new TractorBeamEmitter_Select()
                        {
                            TractorBeamEmitterVhId = tractorBeamEmitterId.VhId,
                            TargetData = _entitySerializationService.Serialize(nodeId, SerializationOptions.Default),
                            Location = node.Transformation.ToLocation()
                        });


                    if (_nodeService.IsHead(in node))
                    {
                        _logger.Verbose("{ClassName}::{MethodName} - Despawning Node {NodeVhId} Tree {TreeId}", nameof(TractorBeamEmitterService), nameof(Select), nodeId.VhId, node.TreeId.VhId);
                        _entitySpawnService.Despawn(sourceId, node.TreeId);
                    }
                    else
                    {
                        _logger.Verbose("{ClassName}::{MethodName} - Despawning Node {NodeVhId}", nameof(TractorBeamEmitterService), nameof(Select), nodeId.VhId);
                        _entitySpawnService.Despawn(sourceId, nodeId);
                    }
                }
                else
                {
                    _logger.Warning("{ClassName}::{MethodName} - Node {NodeId} Tree {TreeId} does not exist", nameof(TractorBeamEmitterService), nameof(Select), nodeId.VhId, node.TreeId.VhId);
                }
            }
            else
            {
                _logger.Warning("{ClassName}::{MethodName} - Node {NodeId} does not exist", nameof(TractorBeamEmitterService), nameof(Select), nodeId.VhId);
            }
        }

        public void Deselect(VhId sourceId, EntityId tractorBeamEmitterId)
        {
            ref Tactical tactical = ref _entityQueryService.QueryById<Tactical>(tractorBeamEmitterId);
            SocketVhId? attachToSocketVhId = _socketService.TryGetClosestOpenSocket(tractorBeamEmitterId, tactical.Target, out NodeSocket nodeSocket)
                ? nodeSocket.Id.VhId : default;

            this.Deselect(sourceId, tractorBeamEmitterId, attachToSocketVhId);
        }

        private Queue<(EntityId id, EntityId headId, Location location)> _deselecteds = new Queue<(EntityId id, EntityId head, Location location)>();
        public void Deselect(VhId sourceId, EntityId tractorBeamEmitterId, SocketVhId? attachToSocketVhId)
        {
            ref var filter = ref this.GetTractorableFilter(tractorBeamEmitterId);
            foreach (var (indices, groupId) in filter)
            {
                var (entityIds, statuses, trees, locations, _) = _entityQueryService.QueryEntities<EntityId, EntityStatus, Tree, Location>(groupId);

                for (int i = 0; i < indices.count; i++)
                {
                    uint index = indices[i];

                    if (statuses[index].IsDespawned)
                    {
                        _logger.Warning("{ClassName}::{MethodName} - Unable to deselect {TractorableId}, despawned. Multiple deselect calls in a single frame?", nameof(TractorBeamEmitterService), nameof(Deselect), entityIds[index].VhId.Value);
                        continue;
                    }

                    EntityId id = entityIds[index];
                    _deselecteds.Enqueue((id, trees[index].HeadId, locations[index]));

                    filter.Remove(id);
                }
            }

            VhId nextSourceId = NameSpace<TractorBeamEmitterService>.Instance.Create(sourceId);
            while (_deselecteds.TryDequeue(out (EntityId id, EntityId headId, Location location) deselected))
            {
                _logger.Verbose("{ClassName}::{MethodName} - Attempting to deselect {TreeId} with emitter {TractorBeamEmitterId}", nameof(TractorBeamEmitterService), nameof(Deselect), deselected.id.VhId.Value, tractorBeamEmitterId.VhId);
                this.Strategy.Publish(new EventDto()
                {
                    SourceId = nextSourceId,
                    Data = new TractorBeamEmitter_Deselect()
                    {
                        TractorBeamEmitterVhId = tractorBeamEmitterId.VhId,
                        TargetData = _entitySerializationService.Serialize(deselected.headId, SerializationOptions.Default),
                        Location = deselected.location,
                        AttachToSocketVhId = attachToSocketVhId
                    }
                });
                _entitySpawnService.Despawn(nextSourceId, deselected.id);
            }
        }

        void IEventEngine<TractorBeamEmitter_Select>.Process(VhId eventId, TractorBeamEmitter_Select data)
        {
            try
            {
                EntityId cloneId = _treeService.Spawn(
                    sourceId: eventId,
                    vhid: eventId.Create(1),
                    belongsToTeam: _teamService.GetDefaultTeamComponent(),
                    treeTypeKey: ChainEntityType.ChainEntityTypeKey,
                    nodes: data.TargetData,
                    initializer: (IEntityService entities, IEntityType entityType, EntityId id, ref EntityInitializer initializer) =>
                    {
                        if (!entities.Query.TryGetId(data.TractorBeamEmitterVhId, out EntityId tractorBeamEmitterId))
                        {
                            throw new ArgumentException($"Unable to locate {nameof(TractorBeamEmitter)} {data.TractorBeamEmitterVhId.Value}");
                        }

                        initializer.Init<Location>(data.Location);
                        initializer.Init<Tractorable>(new Tractorable()
                        {
                            TractorBeamEmitter = tractorBeamEmitterId
                        });
                    });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{ClassName}::{MethodName}<{GenericTypeName}> - Exception thrown", nameof(TractorBeamEmitterService), nameof(Process), nameof(TractorBeamEmitter_Select));
                throw new SimulationOutOfSyncException(ex.Message, ex);
            }
        }

        void IEventEngine<TractorBeamEmitter_Deselect>.Process(VhId eventId, TractorBeamEmitter_Deselect data)
        {
            try
            {
                if (data.AttachToSocketVhId.HasValue && _socketService.TryGetSocket(data.AttachToSocketVhId.Value, out NodeSocket attachToSocket))
                { // Spawn a new piece attached to the input node
                    _socketService.Spawn(eventId, attachToSocket, data.TargetData);
                }
                else
                { // Spawn a new free floating chain
                    EntityId cloneId = _treeService.Spawn(
                        sourceId: eventId,
                        vhid: eventId.Create(2),
                        belongsToTeam: _teamService.GetDefaultTeamComponent(),
                        treeTypeKey: ChainEntityType.ChainEntityTypeKey,
                        nodes: data.TargetData,
                        initializer: (IEntityService entities, IEntityType entityType, EntityId id, ref EntityInitializer initializer) =>
                        {
                            initializer.Init<Location>(data.Location);
                            initializer.Init<Tractorable>(new Tractorable()
                            {
                                TractorBeamEmitter = default
                            });
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{ClassName}::{MethodName}<{GenericTypeName}> - Exception thrown", nameof(TractorBeamEmitterService), nameof(Process), nameof(TractorBeamEmitter_Select));
                throw new SimulationOutOfSyncException(ex.Message, ex);
            }
        }
    }
}
