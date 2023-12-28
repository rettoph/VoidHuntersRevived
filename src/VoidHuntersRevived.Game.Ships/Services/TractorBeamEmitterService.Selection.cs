using Svelto.ECS;
using System.Diagnostics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Options;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Physics.Extensions.FixedPoint;
using VoidHuntersRevived.Common.Pieces;
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
            if (_entities.IsSpawned(nodeId, out GroupIndex nodeGroupIndex))
            {
                ref Node node = ref _entities.QueryByGroupIndex<Node>(nodeGroupIndex);
                if (_entities.IsSpawned(node.TreeId))
                {
                    _logger.Verbose("{ClassName}::{MethodName} - Selecting {NodeId} with TractorBeamEmitter {TractorBeamEmitterId}", nameof(TractorBeamEmitterService), nameof(Select), nodeId.VhId, tractorBeamEmitterId.VhId);

                    this.Simulation.Publish(
                        sender: NameSpace<TractorBeamEmitterService>.Instance,
                        data: new TractorBeamEmitter_Select()
                        {
                            TractorBeamEmitterVhId = tractorBeamEmitterId.VhId,
                            TargetData = _entities.Serialize(nodeId, SerializationOptions.Default),
                            Location = node.Transformation.ToLocation()
                        });


                    if (_nodes.IsHead(in node))
                    {
                        _logger.Verbose("{ClassName}::{MethodName} - Despawning Node {NodeVhId} Tree {TreeId}", nameof(TractorBeamEmitterService), nameof(Select), nodeId.VhId, node.TreeId.VhId);
                        _entities.Despawn(node.TreeId);
                    }
                    else
                    {
                        _logger.Verbose("{ClassName}::{MethodName} - Despawning Node {NodeVhId}", nameof(TractorBeamEmitterService), nameof(Select), nodeId.VhId);
                        _entities.Despawn(nodeId);
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

        public void Deselect(EntityId tractorBeamEmitterId)
        {
            ref Tactical tactical = ref _entities.QueryById<Tactical>(tractorBeamEmitterId);
            SocketVhId? attachToSocketVhId = _sockets.TryGetClosestOpenSocket(tractorBeamEmitterId, tactical.Target, out Socket socket)
                ? socket.Id.VhId : default;

            this.Deselect(tractorBeamEmitterId, attachToSocketVhId);
        }

        public void Deselect(EntityId tractorBeamEmitterId, SocketVhId? attachToSocketVhId)
        {
            ref var filter = ref this.GetTractorableFilter(tractorBeamEmitterId);
            foreach (var (indices, groupId) in filter)
            {
                var (entityIds, entityStatuses, trees, locations, _) = _entities.QueryEntities<EntityId, EntityStatus, Tree, Location>(groupId);

                for (int i = 0; i < indices.count; i++)
                {
                    uint index = indices[i];

                    if (entityStatuses[index].IsDespawned)
                    {
                        _logger.Warning("{ClassName}::{MethodName} - Unable to deselect {TractorableId}, despawned. Multiple deselect calls in a single frame?", nameof(TractorBeamEmitterService), nameof(Deselect), entityIds[index].VhId.Value);
                        continue;
                    }

                    _logger.Verbose("{ClassName}::{MethodName} - Attempting to deselect {TreeId} with emitter {TractorBeamEmitterId}", nameof(TractorBeamEmitterService), nameof(Deselect), entityIds[index].VhId.Value, tractorBeamEmitterId.VhId);
                    this.Simulation.Publish(
                        sender: NameSpace<TractorBeamEmitterService>.Instance,
                        data: new TractorBeamEmitter_Deselect()
                        {
                            TractorBeamEmitterVhId = tractorBeamEmitterId.VhId,
                            TargetData = _entities.Serialize(trees[index].HeadId, SerializationOptions.Default),
                            Location = locations[index],
                            AttachToSocketVhId = attachToSocketVhId
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
                    teamId: Teams.TeamZero,
                    tree: EntityTypes.Chain,
                    nodes: data.TargetData,
                    initializer: (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                    {
                        if (!entities.TryGetId(data.TractorBeamEmitterVhId, out EntityId tractorBeamEmitterId))
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
                if (data.AttachToSocketVhId.HasValue && _sockets.TryGetSocket(data.AttachToSocketVhId.Value, out Socket attachToSocket))
                { // Spawn a new piece attached to the input node
                    _sockets.Spawn(attachToSocket, data.TargetData);
                }
                else
                { // Spawn a new free floating chain
                    EntityId cloneId = _trees.Spawn(
                        vhid: eventId.Create(2),
                        teamId: Teams.TeamZero,
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
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "{ClassName}::{MethodName}<{GenericTypeName}> - Exception thrown", nameof(TractorBeamEmitterService), nameof(Process), nameof(TractorBeamEmitter_Select));
                throw new SimulationOutOfSyncException(ex.Message, ex);
            }
        }
    }
}
