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
        public void Select(VhId sourceId, in Entity<TractorBeamEmitter> tractorBeamEmitter, in Entity<Node> node)
        {
            if (_entityQueryService.IsSpawned(node.GroupIndex) == false)
            {
                _logger.Warning("Node {NodeId} does not exist", node.LocalId);
                return;
            }

            if (_entityQueryService.IsSpawned(node.Component.TreeId) == false)
            {
                _logger.Warning("Node {NodeId} Tree {TreeId} does not exist", node.LocalId, node.Component.TreeId.VhId);
                return;
            }

            _logger.Verbose("Selecting {NodeId} with TractorBeamEmitter {TractorBeamEmitterId}", node.LocalId, tractorBeamEmitter.LocalId);
            this.Strategy.Publish(
                sourceId: NameSpace<TractorBeamEmitterService>.Instance.Create(sourceId),
                data: new TractorBeamEmitter_Select()
                {
                    TractorBeamEmitterGlobalId = tractorBeamEmitter.GlobalId,
                    TargetData = _entitySerializationService.Serialize(node.LocalId, SerializationOptions.Default),
                    Location = node.Component.Transformation.ToLocation()
                });


            if (_nodeService.IsHead(in node.Component))
            {
                _logger.Verbose("Despawning Node {NodeVhId} Tree {TreeId}", node.LocalId, node.Component.TreeId.VhId);
                _entitySpawnService.Despawn(sourceId, node.Component.TreeId);
            }
            else
            {
                _logger.Verbose("Despawning Node {NodeVhId}", node.LocalId);
                _entitySpawnService.Despawn(sourceId, _entityQueryService.GetGlobalId(node.LocalId));
            }
        }

        private readonly Queue<(EntityId id, EntityId headId, Location location)> _deselecteds = new();
        public void Deselect(VhId sourceId, in Entity<TractorBeamEmitter> tractorBeamEmitter, SocketVhId? attachToSocketVhId)
        {
            ref var filter = ref this.GetTractorableFilter(tractorBeamEmitter.LocalId);
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
                _logger.Verbose("Attempting to deselect {TreeId} with emitter {TractorBeamEmitterId}", deselected.id.VhId.Value, tractorBeamEmitter.GlobalId);
                this.Strategy.Publish(new EventDto()
                {
                    SourceId = nextSourceId,
                    Data = new TractorBeamEmitter_Deselect()
                    {
                        TractorBeamEmitterGlobalId = tractorBeamEmitter.GlobalId,
                        TargetData = _entitySerializationService.Serialize(deselected.headId.ToLocalEntityId(), SerializationOptions.Default),
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
                    initializer: (IEntityService entities, in InitializingEntity entity) =>
                    {
                        if (!entities.Query.TryGetLocalId(data.TractorBeamEmitterGlobalId, out EntityLocalId tractorBeamEmitterLocalId))
                        {
                            throw new ArgumentException($"Unable to locate {nameof(TractorBeamEmitter)} {data.TractorBeamEmitterGlobalId.Value}");
                        }

                        entity.Initializer.Init<Location>(data.Location);
                        entity.Initializer.Init<Tractorable>(new Tractorable()
                        {
                            TractorBeamEmitterLocalId = tractorBeamEmitterLocalId
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
                        initializer: (IEntityService entities, in InitializingEntity entity) =>
                        {
                            entity.Initializer.Init<Location>(data.Location);
                            entity.Initializer.Init<Tractorable>(new Tractorable()
                            {
                                TractorBeamEmitterLocalId = default
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
