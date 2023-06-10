using Guppy;
using Guppy.Common;
using Guppy.Providers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Messages;

namespace VoidHuntersRevived.Domain.GameComponents
{
    internal sealed class LaunchComponent : IGameComponent,
        ISubscriber<Launch>
    {
        private readonly IGuppy _guppy;
        private readonly IGuppyProvider _guppies;

        public LaunchComponent(IBus bus, IGuppy guppy, IGuppyProvider guppies)
        {
            _guppy = guppy;
            _guppies = guppies;

            bus.Subscribe(this);
        }

        public void Initialize()
        {
            // throw new NotImplementedException();
        }

        public void Process(in Launch message)
        {
            _guppies.Create(message.GuppyType);
            _guppy.Dispose();
        }
    }
}
