using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Services
{
    [GuppyFilter(typeof(GameGuppy))]
    public interface ITickService : IGameComponent, IUpdateable
    {
        public IEnumerable<Tick> History { get; }

        public Tick? Current { get; }
    }
}
