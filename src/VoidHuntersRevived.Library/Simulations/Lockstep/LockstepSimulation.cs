using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using LiteNetLib;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Attributes;
using VoidHuntersRevived.Common.Services;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Library.Common;
using VoidHuntersRevived.Library.Simulations.Lockstep.Factories;
using VoidHuntersRevived.Library.Simulations.Lockstep.Services;

namespace VoidHuntersRevived.Library.Simulations.Lockstep
{
    [GuppyFilter<GameGuppy>()]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal sealed class LockstepSimulation : Simulation, ISimulation,
        ISubscriber<Tick>,
        ISubscriber<Step>
    {
        private ILockstepEventPublishingService _publisher;
        private IStepService _steps;
        private ISimulationService _simulations;

        public LockstepSimulation(
            ISimulationService simulations, 
            IStepService steps, 
            IFiltered<ILockstepEventPublishingService> publisher, 
            ISimulatedService simulatedEntities) : base(SimulationType.Lockstep, simulatedEntities)
        {
            _simulations = simulations;
            _steps = steps;
            _publisher = publisher.Instance ?? throw new ArgumentNullException();
        }

        public override void Initialize(IServiceProvider provider)
        {
            base.Initialize(provider);

            _publisher.Initialize(base.PublishEvent);
        }

        protected override void Update(GameTime gameTime)
        {
            _steps.Update(gameTime);
        }

        public override void PublishEvent(PeerType source, ISimulationData data)
        {
            _publisher.Publish(source, data);
        }

        public void Process(in Tick message)
        {
            foreach(ISimulationData data in message.Data)
            {
                // At this point in time the data has successfully
                // been converted into lockstep server data,
                // publish it as so.
                _simulations.PublishEvent(PeerType.Server, data);
            }
        }

        public void Process(in Step message)
        {
            this.UpdateSystems(message);
        }
    }
}
