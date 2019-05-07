using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy;
using Guppy.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Players;

namespace VoidHuntersRevived.Library.Scenes
{
    public class VoidHuntersWorldScene : Scene
    {
        protected World world;
        protected LivingObjectCollection<Player> players;

        public VoidHuntersWorldScene(World world, IServiceProvider provider) : base(provider)
        {
            this.world = world;
        }

        protected override void Boot()
        {
            base.Boot();

            this.players = new LivingObjectCollection<Player>(initializeOnAdd: true);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            this.world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);

            this.players.Update(gameTime);

            base.Update(gameTime);
        }
    }
}
