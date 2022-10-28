using Guppy.Common;
using Guppy.MonoGame.Utilities;
using Guppy.Network.Enums;
using Guppy.Resources;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Providers;

namespace VoidHuntersRevived.Library.Services
{
    internal sealed class TickService : ITickService
    {
        private ITickProvider _provider;
        private IList<Tick> _history;
        private IBus _bus;

        public IEnumerable<Tick> History => _history;

        public Tick? Current { get; private set; }

        public TickService(
            IBus bus,
            IFiltered<ITickProvider> providers)
        {

            _history = new List<Tick>();
            _provider = providers.Instance;
            _bus = bus;
        }

        public void Update(GameTime gameTime)
        {
            _provider.Update(gameTime);

            while(_provider.Ready())
            {
                this.Current = _provider.Next();

                _history.Add(this.Current);

                _bus.Publish(this.Current);

                foreach(ITickData data in this.Current)
                {
                    _bus.Publish(data);
                }
            }
        }
    }
}
