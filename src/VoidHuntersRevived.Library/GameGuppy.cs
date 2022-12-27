using Guppy.Attributes;
using Guppy.Common;
using Guppy.MonoGame;
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
using VoidHuntersRevived.Library.Games;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library
{
    public class GameGuppy : FrameableGuppy
    {
        public readonly NetScope NetScope;
        public readonly LockstepSimulation LockstepSimulation;

        public GameGuppy(
            NetScope netScope,
            LockstepSimulation lockstepSimulation,
            IGameComponentService components) : base(components)
        {
            this.NetScope = netScope;
            this.LockstepSimulation = lockstepSimulation;

            this.NetScope.Start(0);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
