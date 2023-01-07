using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using LiteNetLib;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Library.Simulations.Lockstep.Services;

namespace VoidHuntersRevived.Library.Simulations.Lockstep
{
    [GuppyFilter<GameGuppy>()]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal sealed class LockstepSimulation : Simulation<Common.Simulations.Components.Lockstep>, ISimulation,
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
            IParallelService simulatedEntities) : base(SimulationType.Lockstep, simulatedEntities)
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

        public override void PublishEvent(SimulationType source, ISimulationData data)
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
                _simulations.PublishEvent(SimulationType.Lockstep, data);
            }
        }

        public void Process(in Step message)
        {
            this.UpdateSystems(message);
        }
    }
}
