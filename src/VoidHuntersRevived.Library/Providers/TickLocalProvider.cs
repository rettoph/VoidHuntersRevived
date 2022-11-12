using Guppy.Attributes;
using Guppy.Common;
using Guppy.MonoGame.Utilities;
using Guppy.Network.Enums;
using Guppy.Resources;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Factories;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Providers
{
    [AutoSubscribe]
    internal sealed class TickLocalProvider : ITickProvider, ISubscriber<Step>
    {
        private int _id;
        private int _steps;
        private readonly ISetting<int> _stepsPerTick;
        private readonly ITickFactory _factory;

        public TickLocalProvider(ISettingProvider settings, ITickFactory factory)
        {
            _stepsPerTick = settings.Get<int>(SettingConstants.StepsPerTick);
            _id = Tick.MinimumValidId - 1;
            _factory = factory;
        }

        public bool Next([MaybeNullWhen(false)] out Tick next)
        {
            if(_steps >= _stepsPerTick.Value)
            {
                next = _factory.Create(++_id);
                _steps -= _stepsPerTick.Value;
                return true;
            }

            next = null;
            return false;
        }

        public void Process(in Step message)
        {
            _steps++;
        }
    }
}
