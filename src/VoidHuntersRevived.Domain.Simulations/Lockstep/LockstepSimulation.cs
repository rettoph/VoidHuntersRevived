using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using Guppy.Network.Identity;
using Guppy.Resources.Providers;
using LiteNetLib;
using Microsoft.Xna.Framework;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.ECS;
using VoidHuntersRevived.Common.ECS.Factories;
using VoidHuntersRevived.Common.Physics.Factories;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [GuppyFilter<IGameGuppy>()]
    internal abstract class LockstepSimulation : Simulation, ISimulation
    {
        private readonly int _stepsPerTick;
        private int _stepsSinceTick;

        public LockstepSimulation(
            ISettingProvider settings,
            IWorldFactory worldFactory,
            ISpaceFactory spaceFactory) : base(SimulationType.Lockstep, worldFactory, spaceFactory)
        {
            _stepsPerTick = settings.Get<int>(Settings.StepsPerTick).Value;
            _stepsSinceTick = 0;
        }

        protected override bool CanStep(GameTime realTime)
        {
            if (_stepsSinceTick >= _stepsPerTick)
            {
                return false;
            }

            return true;
        }

        protected override bool TryGetNextTick(Tick current, [MaybeNullWhen(false)] out Tick next)
        {
            if (_stepsSinceTick != _stepsPerTick)
            {
                next = null!;
                return false;
            }

            next = null;
            return false;
        }

        public override void Publish(EventDto data)
        {
            this.publisher.Publish(data);
        }

        public void Process(in Step message)
        {
            this.World.Update(message);
        }

        public void Process(in Tick message)
        {
            foreach (EventDto data in message.Events)
            {
                this.Publish(data);
            }
        }
    }
}
