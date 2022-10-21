using Guppy.MonoGame.Utilities;
using Guppy.Network.Enums;
using Guppy.Resources;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Factories;
using VoidHuntersRevived.Library.Models;

namespace VoidHuntersRevived.Library.Providers
{
    internal sealed class LocalTickProvider : ITickProvider
    {
        private uint _id;
        private GuppyTimer _timer;
        private ISetting<int> _tickSpeed;
        private ITickFactory _factory;

        public LocalTickProvider(ISettingProvider settings, ITickFactory factory)
        {
            _tickSpeed = settings.Get<int>(SettingConstants.TickSpeed);
            _timer = new GuppyTimer(TimeSpan.FromMilliseconds(_tickSpeed.Value));
            _id = Tick.MinimumValidId;
            _factory = factory;
        }

        public void Update(GameTime gameTime)
        {
            _timer.Update(gameTime);
        }

        public bool Ready()
        {
            return _timer.Step(out _);
        }

        public Tick Next()
        {
            return _factory.Create(++_id);
        }
    }
}
