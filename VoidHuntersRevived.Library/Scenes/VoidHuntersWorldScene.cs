using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Scenes
{
    public class VoidHuntersWorldScene : Scene
    {
        protected World world;

        public VoidHuntersWorldScene(World world, IServiceProvider provider) : base(provider)
        {
            this.world = world;
        }

        protected override void Initialize()
        {
            base.Initialize();

            var body = BodyFactory.CreateCircle(this.world, 0.5f, 1, Vector2.Zero, BodyType.Dynamic);
        }

        public override void Update(GameTime gameTime)
        {
            this.world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);

            base.Update(gameTime);
        }
    }
}
