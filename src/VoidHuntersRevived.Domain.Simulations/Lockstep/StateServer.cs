using Guppy.Common;
using Guppy.Resources.Providers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Lockstep.Factories;
using VoidHuntersRevived.Common.Simulations.Lockstep.Messages;
using VoidHuntersRevived.Common.Simulations.Lockstep.Providers;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Constants;
using Guppy.Network;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [PeerTypeFilter(PeerType.Server)]
    internal class StateServer : StateDefault
    {
        private TimeSpan _timeSinceStep;
        private readonly ITickFactory _factory;

        public StateServer(
            IBus bus,
            IFiltered<ITickProvider> ticks,
            IFiltered<ITickFactory> factory,
            ISettingProvider settings,
            ILogger log) : base(bus, ticks, settings, log)
        {
            _factory = factory.Instance;
        }

        public override void Enqueue(SimulationEventData input)
        {
            // Create a event data request message and broadcast to the server
            _factory.Enqueue(input);
        }

        public override void Update(GameTime realTime)
        {
            _timeSinceStep += realTime.ElapsedGameTime;

            base.Update(realTime);
        }

        protected override bool CanStep(GameTime realTime)
        {
            return base.CanStep(realTime) && _timeSinceStep >= this.step.ElapsedGameTime;
        }

        protected override void Step(GameTime realTime)
        {
            base.Step(realTime);

            _timeSinceStep -= this.step.ElapsedGameTime;
        }
    }
}
