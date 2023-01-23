using Guppy.Common;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Events;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    internal sealed class TractoringSystem : ParallelEntityProcessingSystem,
        ISubscriber<IInput<StartTractoring>>,
        ISubscriber<IInput<StopTractoring>>
    {
        private static readonly AspectBuilder TractoringAspect = Aspect.All(new[] {
            typeof(Tractoring)
        });

        private readonly ITractorService _tractor;

        private ComponentMapper<Tractoring> _tractorings;
        private ComponentMapper<Tractorable> _tractorables;
        private ComponentMapper<Pilotable> _pilotables;
        private ComponentMapper<Piloting> _pilotings;
        private ComponentMapper<Body> _bodies;
        private ComponentMapper<Tree> _trees;
        private ComponentMapper<Jointable> _jointables;

        public TractoringSystem(ITractorService tractor, ISimulationService simulations) : base(simulations, TractoringAspect)
        {
            _tractor = tractor;

            _tractorings = default!;
            _tractorables = default!;
            _pilotables = default!;
            _pilotings = default!;
            _bodies = default!;
            _trees = default!;
            _jointables = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _tractorings = mapperService.GetMapper<Tractoring>();
            _tractorables = mapperService.GetMapper<Tractorable>();
            _pilotables = mapperService.GetMapper<Pilotable>();
            _pilotings = mapperService.GetMapper<Piloting>();
            _bodies = mapperService.GetMapper<Body>();
            _trees = mapperService.GetMapper<Tree>();
            _jointables = mapperService.GetMapper<Jointable>();
        }

        public void Process(in IInput<StartTractoring> message)
        {
            if (!message.Simulation.TryGetEntityId(message.Data.Tractorable, out int tractorableId))
            {
                return;
            }

            if (!_tractorables.Has(tractorableId))
            {
                throw new NotImplementedException();
            }

            var piloting = _pilotings.Get(message.UserId);
            _tractorings.Put(piloting.Pilotable.Id, new Tractoring(piloting.Pilotable.Id, tractorableId));
        }

        public void Process(in IInput<StopTractoring> message)
        {
            var piloting = _pilotings.Get(message.UserId);

            if(!_tractorings.TryGet(piloting.Pilotable.Id, out var tractoring))
            {
                return;
            }

            _tractorings.Delete(piloting.Pilotable.Id);

            if(!_tractor.TransformTractorable(message.Data.Target, tractoring, out var potential))
            {
                return;
            }

            message.Simulation.PublishEvent(new DestroyEntity()
            {
                EntityKey = potential.Joint.Entity.Get<Node>().Tree.Get<Parallelable>().Key
            });

            message.Simulation.PublishEvent(new CreateJointing()
            {
                Parent = potential.Parent.Entity.Get<Parallelable>().Key,
                ParentJointId = potential.Parent.Index,
                Joint = potential.Joint.Entity.Get<Parallelable>().Key,
                ChildJointId = potential.Joint.Index
            });
        }

        protected override void Process(ISimulation simulation, GameTime gameTime, int entityId)
        {
            var pilotable = _pilotables.Get(entityId);
            var tractoring = _tractorings.Get(entityId);

            _tractor.TransformTractorable(pilotable.Aim.Value, tractoring, out _);
        }
    }
}
