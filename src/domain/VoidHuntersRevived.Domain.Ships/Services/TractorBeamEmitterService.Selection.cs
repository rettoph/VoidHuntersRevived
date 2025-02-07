using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Common.Exceptions;

namespace VoidHuntersRevived.Domain.Ships.Services
{
    public partial class TractorBeamEmitterService : ITractorBeamEmitterService,
        IEventSystem<TractorBeamEmitter_Select>,
        IEventSystem<TractorBeamEmitter_Deselect>
    {
        public void Select(VhId sourceId, EntityGlobalId tractorBeamEmitterGlobalId, EntityGlobalId nodeGlobalId)
        {
            if (this._entityQueryService.IsSpawned(nodeGlobalId, out GroupIndex nodeGroupIndex) == false)
            {
                this._logger.Warning("Node {NodeGlobalId} does not exist", nodeGlobalId);
                return;
            }

            if (this._entityQueryService.TryQueryByGroupIndex<Node>(nodeGroupIndex, out Node node) == false)
            {
                this._logger.Warning("Node {NodeGlobalId} is not a valid Node", nodeGlobalId);
                return;
            }

            if (this._entityQueryService.TryQueryByGroupIndex<Fixture>(nodeGroupIndex, out Fixture fixture) == false)
            {
                this._logger.Warning("Node {NodeGlobalId} is not a valid Fixture", nodeGlobalId);
                return;
            }

            if (this._entityQueryService.IsSpawned(node.TreeLocalId) == false)
            {
                this._logger.Warning("Node {NodeGlobalId} Tree {TreeLocalId} does not exist", nodeGlobalId, node.TreeLocalId);
                return;
            }

            this._logger.Verbose("Selecting {NodeGlobalId} with TractorBeamEmitter {TractorBeamEmitterGlobalId}", nodeGlobalId, tractorBeamEmitterGlobalId);
            this.Strategy.Publish(
                sourceId: NameSpace<TractorBeamEmitterService>.Instance.Create(sourceId),
                data: new TractorBeamEmitter_Select()
                {
                    TractorBeamEmitterGlobalId = tractorBeamEmitterGlobalId,
                    TargetData = this._entitySerializationService.Serialize(nodeGroupIndex.GroupID, nodeGroupIndex.Index, SerializationOptions.Default),
                    Transform = fixture.WorldTransform
                });


            if (this._nodeService.IsHead(in node))
            {
                this._logger.Verbose("Despawning Node {NodeGlobalId} Tree {TreeLocalId}", nodeGlobalId, node.TreeLocalId);
                this._entitySpawnService.Despawn(sourceId, node.TreeLocalId);
            }
            else
            {
                this._logger.Verbose("Despawning Node {NodeGlobalId}", nodeGlobalId);
                this._entitySpawnService.Despawn(sourceId, nodeGlobalId);
            }
        }

        private readonly Queue<(EntityLocalId localId, EntityLocalId headLocalId, Body body)> _deselecteds = new();
        public void Deselect(VhId sourceId, EntityGlobalId tractorBeamEmitterGlobalId, NodeSocketGlobalId? attachToSocketVhId)
        {
            if (this._entityQueryService.TryGetLocalId(tractorBeamEmitterGlobalId, out EntityLocalId tracorBeamEmitterLocalId) == false)
            {
                throw new NotImplementedException();
            }

            ref var filter = ref this._entityQueryService.GetFilter<TractorBeamEmitter, Tractorable>(tracorBeamEmitterLocalId);
            foreach (var (indices, groupId) in filter)
            {
                var (localIds, statuses, trees, transforms, _) = this._entityQueryService.QueryEntities<EntityLocalId, EntityStatus, Tree, Body>(groupId);

                for (int i = 0; i < indices.count; i++)
                {
                    uint index = indices[i];
                    EntityLocalId localId = localIds[index];

                    if (statuses[index].IsDespawned)
                    {
                        this._logger.Warning("Unable to deselect {TractorableId}, despawned. Multiple deselect calls in a single frame?", localId);
                        continue;
                    }


                    this._deselecteds.Enqueue((localId, trees[index].HeadLocalId, transforms[index]));

                    filter.Remove(localId);
                }
            }

            VhId nextSourceId = NameSpace<TractorBeamEmitterService>.Instance.Create(sourceId);
            while (this._deselecteds.TryDequeue(out (EntityLocalId localId, EntityLocalId headLocalId, Body body) deselected))
            {
                this._logger.Verbose("Attempting to deselect {TreeId} with emitter {TractorBeamEmitterLocalId}", deselected.localId, tractorBeamEmitterGlobalId);
                this.Strategy.Publish(new EventDto()
                {
                    SourceId = nextSourceId,
                    Data = new TractorBeamEmitter_Deselect()
                    {
                        TractorBeamEmitterGlobalId = tractorBeamEmitterGlobalId,
                        TargetData = this._entitySerializationService.Serialize(deselected.headLocalId, SerializationOptions.Default),
                        Transform = deselected.body.Transform,
                        AttachToSocketVhId = attachToSocketVhId
                    }
                });
                this._entitySpawnService.Despawn(nextSourceId, deselected.localId);
            }
        }

        void IEventSystem<TractorBeamEmitter_Select>.Process(VhId eventId, TractorBeamEmitter_Select data)
        {
            try
            {
                EntityLocalId cloneLocalId = this._treeService.Spawn(
                    sourceId: eventId,
                    globalId: eventId.ToGlobalEntityId(1),
                    team: this._teamService.GetDefaultTeam(),
                    treeTemplateKey: Resources.EntityTemplates.Ship.ChainEntityTemplate,
                    nodes: data.TargetData,
                    initializer: (IEntityService entities, in InitializingEntity entity) =>
                    {
                        if (!entities.Query.TryGetLocalId(data.TractorBeamEmitterGlobalId, out EntityLocalId tractorBeamEmitterLocalId))
                        {
                            throw new ArgumentException($"Unable to locate {nameof(TractorBeamEmitter)} {data.TractorBeamEmitterGlobalId.Value}");
                        }

                        entity.Initializer.Init<Body>(new Body(entity.LocalId, data.Transform));
                        entity.Initializer.Init<Tractorable>(new Tractorable(tractorBeamEmitterLocalId));
                    });
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, "Exception thrown");
#if DEBUG
                throw;
#endif
                throw new SimulationOutOfSyncException(ex.Message, ex);
            }
        }

        void IEventSystem<TractorBeamEmitter_Deselect>.Process(VhId eventId, TractorBeamEmitter_Deselect data)
        {
            try
            {
                if (data.AttachToSocketVhId.HasValue && this._socketService.TryGetNodeSocket(data.AttachToSocketVhId.Value, out NodeSocket attachToSocket))
                { // Spawn a new piece attached to the input node
                    this._socketService.Spawn(eventId, attachToSocket, data.TargetData);
                }
                else
                { // Spawn a new free floating chain
                    EntityLocalId cloneLocalId = this._treeService.Spawn(
                        sourceId: eventId,
                        globalId: eventId.ToGlobalEntityId(2),
                        team: this._teamService.GetDefaultTeam(),
                        treeTemplateKey: Resources.EntityTemplates.Ship.ChainEntityTemplate,
                        nodes: data.TargetData,
                        initializer: (IEntityService entities, in InitializingEntity entity) =>
                        {
                            entity.Initializer.Init<Body>(new Body(entity.LocalId, data.Transform));
                            entity.Initializer.Init<Tractorable>(new Tractorable(default));
                        });
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, "Exception thrown");
                throw new SimulationOutOfSyncException(ex.Message, ex);
            }
        }
    }
}