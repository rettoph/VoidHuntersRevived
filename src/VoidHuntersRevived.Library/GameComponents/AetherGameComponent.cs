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
    internal sealed class AetherGameComponent : IGameComponent, ISubscriber<Step>
    {
        private readonly AetherWorld _aether;

        public AetherGameComponent(AetherWorld aether)
        {
            _aether = aether;
        }

        public void Initialize()
        {
            // throw new NotImplementedException();
        }

        public void Process(in Step message)
        {
            _aether.Step(message.Interval);
        }
    }
}
