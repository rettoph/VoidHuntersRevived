using Guppy.Attributes;
using Guppy.Common;
using Guppy.MonoGame.Utilities;
using Guppy.Resources;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.GameComponents
{
    [AutoSubscribe]
    internal sealed class AetherGameComponent : SimpleGameComponent, ISubscriber<AetherStep>, ISubscriber<Tick>
    {
        private int _aetherStepsSinceTick;
        private readonly IBus _bus;
        private readonly ISetting<int> _worldStepsPerTick;
        private readonly ISetting<int> _tickSpeed;
        private readonly GuppyTimer _aetherStepTimer;
        private readonly AetherStep _aetherStep;
        private readonly AetherWorld _aether;

        public AetherGameComponent(AetherWorld aether, ISettingProvider settings, IBus bus)
        {
            _bus = bus;
            _aether = aether;
            _worldStepsPerTick = settings.Get<int>(SettingConstants.WorldStepsPerTick);
            _tickSpeed = settings.Get<int>(SettingConstants.TickSpeed);
            _aetherStepsSinceTick = 0;
            _aetherStepTimer = new GuppyTimer()
            {
                // Calculated by the tick speed divided by the number of steps per tick. the .99 multiplier is to
                // make the world run slightly fast, but not perceptibly so.
                Interval = TimeSpan.FromMilliseconds(_tickSpeed.Value) / _worldStepsPerTick.Value * 0.99f
            };
            _aetherStep = new AetherStep(_aetherStepTimer.Interval);

            _aether.CreateRectangle(10, 1, 1, new Vector2(0, 2), 0, AetherBodyType.Static);
        }

        public override void Update(GameTime gameTime)
        {
            _aetherStepTimer.Update(gameTime);

            while (_aetherStepTimer.Step(out _))
            {
                if (_aetherStepsSinceTick < _worldStepsPerTick.Value)
                {
                    _bus.Publish(_aetherStep);
                }
            }
        }

        public void Process(in AetherStep message)
        {
            this.Step(in message.Interval);
        }

        public void Process(in Tick message)
        {
            while (_aetherStepsSinceTick < _worldStepsPerTick.Value)
            {
                this.Step(in _aetherStep.Interval);
            }

            _aetherStepsSinceTick = 0;
            _aetherStepTimer.ElapsedTime = TimeSpan.Zero;
        }

        private void Step(in TimeSpan interval)
        {
            _aether.Step(interval);
            _aetherStepsSinceTick++;
        }
    }
}
