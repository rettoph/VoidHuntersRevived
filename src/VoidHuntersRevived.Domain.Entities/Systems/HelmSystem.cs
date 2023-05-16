using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    internal sealed class HelmSystem : BasicSystem,
        ISimulationEventListener<SetHelmDirection>
    {
        private ComponentMapper<Helm> _helms = null!;

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _helms = world.ComponentMapper.GetMapper<Helm>();
        }

        public void Process(ISimulationEvent<SetHelmDirection> @event)
        {
            if (!@event.Simulation.TryGetEntityId(@event.Body.HelmKey, out int helmId))
            {
                return;
            }

            if (!_helms.TryGet(helmId, out Helm? helm))
            {
                return;
            }

            if (@event.Body.Value && (helm.Direction & @event.Body.Which) == 0)
            {
                helm.Direction |= @event.Body.Which;
                return;
            }

            if (!@event.Body.Value && (helm.Direction & @event.Body.Which) != 0)
            {
                helm.Direction &= ~@event.Body.Which;
                return;
            }
        }
    }
}
