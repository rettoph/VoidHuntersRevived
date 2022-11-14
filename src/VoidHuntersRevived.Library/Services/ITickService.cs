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
using VoidHuntersRevived.Library.Providers;

namespace VoidHuntersRevived.Library.Services
{
    [GuppyFilter(typeof(GameGuppy))]
    public interface ITickService
    {
        IList<Tick> History { get; }

        Tick? Current { get; }

        int LastId { get; }

        bool Next();
    }
}
