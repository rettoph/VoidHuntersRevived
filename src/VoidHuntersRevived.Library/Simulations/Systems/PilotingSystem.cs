using Guppy.Common;
using Guppy.MonoGame;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Messages;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Library.Simulations.Systems
{
    internal sealed class PilotingSystem : EntitySystem,
        ISubscriber<ISimulationEvent<SetPilotingDirection>>
    {
        private ComponentMapper<Piloting> _pilotings;

        public PilotingSystem() : base(Aspect.All(typeof(Piloting)))
        {
            _pilotings = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _pilotings= mapperService.GetMapper<Piloting>();
        }

        public void Process(in ISimulationEvent<SetPilotingDirection> message)
        {
            var pilotId = message.Simulation.GetEntityId(message.Data.PilotKey);
            var piloting = _pilotings.Get(pilotId);
            var pilotable = piloting?.Pilotable;

            if(pilotable is null)
            {
                return;
            }

            if (message.Data.Value && (pilotable.Direction & message.Data.Which) == 0)
            {
                pilotable.Direction |= message.Data.Which;
                return;
            }

            if (!message.Data.Value && (pilotable.Direction & message.Data.Which) != 0)
            {
                pilotable.Direction &= ~message.Data.Which;
                return;
            }
        }
    }
}
