using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Physics.Common.Extensions.FixedPoint;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Components;
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
                    _logger.Verbose("Selecting {NodeId} with TractorBeamEmitter {TractorBeamEmitterId}", nodeId.VhId, tractorBeamEmitterId.VhId);

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
                        _logger.Verbose("Despawning Node {NodeVhId} Tree {TreeId}", nodeId.VhId, node.TreeId.VhId);
                        _entitySpawnService.Despawn(sourceId, node.TreeId);
                    }
                    else
                    {
                        _logger.Verbose("Despawning Node {NodeVhId}", nodeId.VhId);
                        _entitySpawnService.Despawn(sourceId, nodeId);
                    }
                }
                else
                {
                    _logger.Warning("Node {NodeId} Tree {TreeId} does not exist", nodeId.VhId, node.TreeId.VhId);
                }
            }
            else
            {
                _logger.Warning("Node {NodeId} does not exist", nodeId.VhId);
            }
        }

        public void Deselect(VhId sourceId, EntityId tractorBeamEmitterId)
        {
            ref Tactical tactical = ref _entityQueryService.QueryById<Tactical>(tractorBeamEmitterId);
            SocketVhId? attachToSocketVhId = _socketService.TryGetClosestOpenSocket(tractorBeamEmitterId, tactical.Target, out NodeSocket nodeSocket)
                ? nodeSocket.Id.VhId : default;

            this.Deselect(sourceId, tractorBeamEmitterId, attachToSocketVhId);
        }

        private readonly Queue<(EntityId id, EntityId headId, Location location)> _deselecteds = new();
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
                        _logger.Warning("Unable to deselect {TractorableId}, despawned. Multiple deselect calls in a single frame?", entityIds[index].VhId.Value);
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
                _logger.Verbose("Attempting to deselect {TreeId} with emitter {TractorBeamEmitterId}", deselected.id.VhId.Value, tractorBeamEmitterId.VhId);
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
                    globalId: eventId.ToGlobalEntityId(1),
                    team: _teamService.GetDefaultTeam(),
                    treeTemplateKey: Resources.EntityTemplates.Ship.ChainEntityTemplate,
                    nodes: data.TargetData,
                    initializer: (IEntityService entities, IEntityTemplate entityTemplate, EntityId id, ref EntityInitializer initializer) =>
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
                _logger.Error(ex, "Exception thrown");
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
                        globalId: eventId.ToGlobalEntityId(2),
                        team: _teamService.GetDefaultTeam(),
                        treeTemplateKey: Resources.EntityTemplates.Ship.ChainEntityTemplate,
                        nodes: data.TargetData,
                        initializer: (IEntityService entities, IEntityTemplate entityTemplate, EntityId id, ref EntityInitializer initializer) =>
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
                _logger.Error(ex, "Exception thrown");
                throw new SimulationOutOfSyncException(ex.Message, ex);
            }
        }
    }
}
