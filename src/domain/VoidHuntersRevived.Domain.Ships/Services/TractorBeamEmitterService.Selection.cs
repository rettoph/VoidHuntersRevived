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
        public void Select(VhId sourceId, EntityGlobalId tractorBeamEmitterGlobalId, EntityGlobalId nodeGlobalId)
        {
            if (_entityQueryService.IsSpawned(nodeGlobalId, out GroupIndex nodeGroupIndex) == false)
            {
                _logger.Warning("Node {NodeGlobalId} does not exist", nodeGlobalId);
                return;
            }

            if (_entityQueryService.TryQueryByGroupIndex<Node>(nodeGroupIndex, out Node node) == false)
            {
                _logger.Warning("Node {NodeGlobalId} is not a valid Node", nodeGlobalId);
                return;
            }

            if (_entityQueryService.IsSpawned(node.TreeLocalId) == false)
            {
                _logger.Warning("Node {NodeGlobalId} Tree {TreeLocalId} does not exist", nodeGlobalId, node.TreeLocalId);
                return;
            }

            _logger.Verbose("Selecting {NodeGlobalId} with TractorBeamEmitter {TractorBeamEmitterGlobalId}", nodeGlobalId, tractorBeamEmitterGlobalId);
            this.Strategy.Publish(
                sourceId: NameSpace<TractorBeamEmitterService>.Instance.Create(sourceId),
                data: new TractorBeamEmitter_Select()
                {
                    TractorBeamEmitterGlobalId = tractorBeamEmitterGlobalId,
                    TargetData = _entitySerializationService.Serialize(nodeGroupIndex.GroupID, nodeGroupIndex.Index, SerializationOptions.Default),
                    Location = node.Transformation.ToLocation()
                });


            if (_nodeService.IsHead(in node))
            {
                _logger.Verbose("Despawning Node {NodeGlobalId} Tree {TreeLocalId}", nodeGlobalId, node.TreeLocalId);
                _entitySpawnService.Despawn(sourceId, node.TreeLocalId);
            }
            else
            {
                _logger.Verbose("Despawning Node {NodeGlobalId}", nodeGlobalId);
                _entitySpawnService.Despawn(sourceId, nodeGlobalId);
            }
        }

        private readonly Queue<(EntityLocalId localId, EntityLocalId headLocalId, Location location)> _deselecteds = new();
        public void Deselect(VhId sourceId, EntityGlobalId tractorBeamEmitterGlobalId, NodeSocketGlobalId? attachToSocketVhId)
        {
            if (_entityQueryService.TryGetLocalId(tractorBeamEmitterGlobalId, out EntityLocalId tracorBeamEmitterLocalId) == false)
            {
                throw new NotImplementedException();
            }

            ref var filter = ref this.GetTractorableFilter(tracorBeamEmitterLocalId);
            foreach (var (indices, groupId) in filter)
            {
                var (localIds, statuses, trees, locations, _) = _entityQueryService.QueryEntities<EntityLocalId, EntityStatus, Tree, Location>(groupId);

                for (int i = 0; i < indices.count; i++)
                {
                    uint index = indices[i];
                    EntityLocalId localId = localIds[index];

                    if (statuses[index].IsDespawned)
                    {
                        _logger.Warning("Unable to deselect {TractorableId}, despawned. Multiple deselect calls in a single frame?", localId);
                        continue;
                    }


                    _deselecteds.Enqueue((localId, trees[index].HeadLocalId, locations[index]));

                    filter.Remove(localId);
                }
            }

            VhId nextSourceId = NameSpace<TractorBeamEmitterService>.Instance.Create(sourceId);
            while (_deselecteds.TryDequeue(out (EntityLocalId localId, EntityLocalId headLocalId, Location location) deselected))
            {
                _logger.Verbose("Attempting to deselect {TreeId} with emitter {TractorBeamEmitterLocalId}", deselected.localId, tractorBeamEmitterGlobalId);
                this.Strategy.Publish(new EventDto()
                {
                    SourceId = nextSourceId,
                    Data = new TractorBeamEmitter_Deselect()
                    {
                        TractorBeamEmitterGlobalId = tractorBeamEmitterGlobalId,
                        TargetData = _entitySerializationService.Serialize(deselected.headLocalId, SerializationOptions.Default),
                        Location = deselected.location,
                        AttachToSocketVhId = attachToSocketVhId
                    }
                });
                _entitySpawnService.Despawn(nextSourceId, deselected.localId);
            }
        }

        void IEventEngine<TractorBeamEmitter_Select>.Process(VhId eventId, TractorBeamEmitter_Select data)
        {
            try
            {
                EntityLocalId cloneLocalId = _treeService.Spawn(
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
                if (data.AttachToSocketVhId.HasValue && _socketService.TryGetNodeSocket(data.AttachToSocketVhId.Value, out NodeSocket attachToSocket))
                { // Spawn a new piece attached to the input node
                    _socketService.Spawn(eventId, attachToSocket, data.TargetData);
                }
                else
                { // Spawn a new free floating chain
                    EntityLocalId cloneLocalId = _treeService.Spawn(
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
