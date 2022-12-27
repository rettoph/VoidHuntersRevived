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
    public sealed class ClientGameGuppy : GameGuppy, IDisposable
    {
        public ClientGameGuppy(NetScope netScope, LockstepSimulation lockstepSimulation, IGameComponentService components) : base(netScope, lockstepSimulation, components)
        {
        }

        public void Dispose()
        {
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.LockstepSimulation.World.Draw(gameTime);
        }
    }
}
