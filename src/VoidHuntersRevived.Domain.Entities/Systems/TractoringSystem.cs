using Guppy.Common;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Simulations;
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

        private ComponentMapper<Tractoring> _tractorings;
        private ComponentMapper<Tractorable> _tractorables;
        private ComponentMapper<Body> _bodies;
        private ComponentMapper<Pilotable> _pilotables;
        private ComponentMapper<Piloting> _pilotings;

        public TractoringSystem(ISimulationService simulations) : base(simulations, TractoringAspect)
        {
            _tractorings = default!;
            _bodies = default!;
            _pilotables = default!;
            _tractorables = default!;
            _pilotings = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _tractorings = mapperService.GetMapper<Tractoring>();
            _bodies = mapperService.GetMapper<Body>();
            _pilotables = mapperService.GetMapper<Pilotable>();
            _tractorables = mapperService.GetMapper<Tractorable>();
            _pilotings = mapperService.GetMapper<Piloting>();
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
            _tractorings.Put(piloting.Pilotable.Id, new Tractoring(tractorableId));
        }

        public void Process(in IInput<StopTractoring> message)
        {
            var piloting = _pilotings.Get(message.UserId);

            if(!_tractorings.TryGet(piloting.Pilotable.Id, out var tractoring))
            {
                return;
            }

            var body = _bodies.Get(tractoring.TractorableId);
            this.TransformBody(body, message.Data.Target);

            _tractorings.Delete(piloting.Pilotable.Id);
        }

        protected override void Process(ISimulation simulation, GameTime gameTime, int entityId)
        {
            var pilotable = _pilotables.Get(entityId);
            var tractoring = _tractorings.Get(entityId);
            var body = _bodies.Get(tractoring.TractorableId);

            this.TransformBody(body, pilotable.Aim.Value);
        }

        private void TransformBody(Body body, Vector2 target)
        {
            target = Vector2.Transform(target, body.GetLocalCenterTransformation().Invert());
            body.SetTransformIgnoreContacts(target, body.Rotation);
        }
    }
}
