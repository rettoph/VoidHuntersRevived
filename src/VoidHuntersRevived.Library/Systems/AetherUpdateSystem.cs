using Guppy.Attributes;
using Guppy.Common;
using Guppy.Resources;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Systems
{
    [AutoSubscribe]
    [GuppyFilter(typeof(GameGuppy))]
    internal sealed class AetherUpdateSystem : ISystem, ISubscriber<Tick>
    {
        private readonly AetherWorld _aether;
        private readonly ISetting<int> _tickSpeed;

        public AetherUpdateSystem(AetherWorld aether, ISettingProvider settings)
        {
            _aether = aether;
            _tickSpeed = settings.Get<int>(SettingConstants.TickSpeed);
        }

        public void Initialize(ECSWorld world)
        {
            // throw new NotImplementedException();

            _aether.CreateRectangle(10, 1, 1, new Vector2(0, 2), 0, BodyType.Static);
        }
        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        public void Process(in Tick message)
        {
            var interval = TimeSpan.FromMilliseconds(_tickSpeed.Value);

            _aether.Step(interval);
        }
    }
}
