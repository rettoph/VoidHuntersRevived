using Guppy.Common;
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
        ISubscriber<ISimulationEvent<SetHelmDirection>>
    {
        private ComponentMapper<Helm> _helms = null!;

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _helms = world.ComponentManager.GetMapper<Helm>();
        }

        public void Process(in ISimulationEvent<SetHelmDirection> message)
        {
            if (!message.Simulation.TryGetEntityId(message.Body.HelmKey, out int helmId))
            {
                return;
            }

            if (!_helms.TryGet(helmId, out Helm? helm))
            {
                return;
            }

            if (message.Body.Value && (helm.Direction & message.Body.Which) == 0)
            {
                helm.Direction |= message.Body.Which;
                return;
            }

            if (!message.Body.Value && (helm.Direction & message.Body.Which) != 0)
            {
                helm.Direction &= ~message.Body.Which;
                return;
            }
        }
    }
}
