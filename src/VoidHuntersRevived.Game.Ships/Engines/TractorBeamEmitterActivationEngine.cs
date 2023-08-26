using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Ships.Events;
using VoidHuntersRevived.Common.Entities.Services;
using Serilog;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Game.Common;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Game.Ships.Engines
{
    [AutoLoad]
    internal class TractorBeamEmitterActivationEngine : BasicEngine,
        IEventEngine<TractorBeamEmitter_TryActivate>,
        IEventEngine<TractorBeamEmitter_Activate>,
        IRevertEventEngine<TractorBeamEmitter_Activate>
    {
        private readonly IEntityService _entities;
        private readonly ITreeService _trees;
        private readonly ILogger _logger;

        public TractorBeamEmitterActivationEngine(
            IEntityService entities,
            ITreeService trees,
            ILogger logger)
        {
            _entities = entities;
            _trees = trees;
            _logger = logger;
        }

        public void Process(VhId eventId, TractorBeamEmitter_TryActivate data)
        {
            if (!_entities.TryGetId(data.TargetVhId, out EntityId targetNodeId) || _entities.GetState(targetNodeId) != EntityState.Spawned)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - TargetVhId {TargetId} not found.", nameof(TractorBeamEmitterActivationEngine), nameof(Process), nameof(TractorBeamEmitter_TryActivate), data.TargetVhId.Value);
                return;
            }

            Node targetNode = _entities.QueryById<Node>(targetNodeId);

            if (!_entities.TryGetId(targetNode.TreeId.VhId, out EntityId targetTreeId) || _entities.GetState(targetTreeId) != EntityState.Spawned)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - TreeId {TargetTreeId} not found.", nameof(TractorBeamEmitterActivationEngine), nameof(Process), nameof(TractorBeamEmitter_TryActivate), targetNode.TreeId.VhId.Value);
                return;
            }

            this.Simulation.Publish(eventId, new TractorBeamEmitter_Activate()
            {
                TractorBeamEmitterVhId = data.ShipVhId,
                TargetData = _entities.Serialize(targetNodeId)
            });

            Tree targetTree = _entities.QueryById<Tree>(targetTreeId);
            _entities.Despawn(targetTreeId);
        }

        public void Process(VhId eventId, TractorBeamEmitter_Activate data)
        {
            EntityId tractorBeamEmitterId = _entities.GetId(data.TractorBeamEmitterVhId);

            ref TractorBeamEmitter tractorBeamEmitter = ref _entities.QueryById<TractorBeamEmitter>(tractorBeamEmitterId, out GroupIndex groupIndex);
            ref Tactical tactical = ref _entities.QueryByGroupIndex<Tactical>(groupIndex);

            if (tractorBeamEmitter.Active)
            {
                this.Simulation.Publish(eventId, new TractorBeamEmitter_TryDeactivate()
                {
                    ShipVhId = data.TractorBeamEmitterVhId,
                    TargetVhId = tractorBeamEmitter.TargetId.VhId
                });
            }

            EntityId cloneId = _trees.Spawn(
                vhid: eventId.Create(1),
                tree: EntityTypes.Chain,
                nodes: data.TargetData,
                initializer: (IEntityService entities, ref EntityInitializer initializer, in EntityId id) =>
                {
                    initializer.Init<Tractorable>(new Tractorable()
                    {
                        IsTractored = true
                    });
                });

            tractorBeamEmitter.TargetId = cloneId;
            tractorBeamEmitter.Active = true;
            tactical.AddUse();

            _logger.Verbose("{ClassName}::{MethodName}<{GenericTypeName}> - TractorBeam {TractorBeamId} has selected {TargetId}", nameof(TractorBeamEmitterActivationEngine), nameof(Process), nameof(TractorBeamEmitter_Activate), tractorBeamEmitterId.VhId.Value, tractorBeamEmitter.TargetId.VhId.Value);
        }

        public void Revert(VhId eventId, TractorBeamEmitter_Activate data)
        {
            EntityId tractorBeamEmitterId = _entities.GetId(data.TractorBeamEmitterVhId);

            ref TractorBeamEmitter tractorBeamEmitter = ref _entities.QueryById<TractorBeamEmitter>(tractorBeamEmitterId, out GroupIndex groupIndex);
            ref Tactical tactical = ref _entities.QueryByGroupIndex<Tactical>(groupIndex);

            if (tractorBeamEmitter.TargetId.VhId.Value != eventId.Create(1).Value)
            {
                _logger.Warning("{ClassName}::{MethodName}<{GenericTypeName}> - Target is no longer {OldTargetId}, now {NewTargetId}", nameof(TractorBeamEmitterActivationEngine), nameof(Revert), nameof(TractorBeamEmitter_Activate), eventId.Create(1).Value, tractorBeamEmitter.TargetId.VhId.Value);
                return;
            }

            tactical.RemoveUse();
            tractorBeamEmitter.Active = false;
            tractorBeamEmitter.TargetId = default;
        }
    }
}
