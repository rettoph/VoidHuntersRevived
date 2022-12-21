using Guppy.Attributes;
using Guppy.Common;
using Guppy.Resources;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Subscribers
{
    [GuppyFilter(typeof(GameGuppy))]
    internal sealed class WorldTickSubscriber : ISubscriber<Tick>
    {
        private readonly ECSWorld _world;
        private readonly ISetting<TimeSpan> _stepInterval;
        private readonly ISetting<int> _stepsPerTick;
        private readonly GameTime _gameTime;

        public WorldTickSubscriber(ECSWorld world, ISettingProvider settings)
        {
            _world = world;
            _stepInterval = settings.Get<TimeSpan>(SettingConstants.StepInterval);
            _stepsPerTick = settings.Get<int>(SettingConstants.StepsPerTick);
            _gameTime = new GameTime(TimeSpan.Zero, TimeSpan.Zero);
        }

        public void Process(in Tick message)
        {
            _gameTime.ElapsedGameTime = _stepInterval.Value * _stepsPerTick.Value;
            _gameTime.TotalGameTime += _gameTime.ElapsedGameTime;

            _world.Update(_gameTime);
        }
    }
}
