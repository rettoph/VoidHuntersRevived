using Guppy.Common;
using Guppy.Common.Providers;
using Guppy.ECS.Providers;
using Guppy.Network;
using Guppy.Resources;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Simulations;

namespace VoidHuntersRevived.Library.Games
{
    public sealed class LockstepSimulation : Simulation, 
        ISubscriber<Tick>
    {
        private readonly ISetting<TimeSpan> _stepInterval;
        private readonly ISetting<int> _stepsPerTick;
        private readonly GameTime _gameTime;

        public override SimulationType Type => SimulationType.Lockstep;

        public override AetherWorld Aether => throw new NotImplementedException();

        public LockstepSimulation(SimulationState state, IWorldProvider worldProvider, IFilteredProvider filteredProvider, ISettingProvider settings) : base(worldProvider, filteredProvider)
        {
            _stepInterval = settings.Get<TimeSpan>(SettingConstants.StepInterval);
            _stepsPerTick = settings.Get<int>(SettingConstants.StepsPerTick);
            _gameTime = new GameTime(TimeSpan.Zero, TimeSpan.Zero);
        }

        public void Process(in Tick message)
        {
            this.Publish(message);

            foreach(ISimulationEvent eventData in message.Events)
            {
                this.Publish(eventData);
            }

            _gameTime.ElapsedGameTime = _stepInterval.Value * _stepsPerTick.Value;
            _gameTime.TotalGameTime += _gameTime.ElapsedGameTime;

            this.World.Update(_gameTime);
        }
    }
}
