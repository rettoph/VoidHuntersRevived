using Guppy.Common;
using Guppy.MonoGame.Services;
using Guppy.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Games;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Client.Library
{
    public sealed class ClientGameGuppy : GameGuppy
    {
        public ClientGameGuppy(World world, NetScope netScope, LockstepSimulation lockstepSimulation, IGameComponentService components) : base(world, netScope, lockstepSimulation, components)
        {
        }
    }
}
