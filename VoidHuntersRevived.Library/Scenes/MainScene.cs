using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Library.Scenes
{
    /// <summary>
    /// The main scene will manage actual gameplay within the game
    /// </summary>
    public class MainScene : Scene
    {
        public World World { get; set; }

        public MainScene(IServiceProvider provider, IGame game) : base(provider, game)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Create a new farseer world
            this.World = new World(Vector2.Zero);

            var floor = BodyFactory.CreateBody(this.World, Vector2.Zero, 0, BodyType.Dynamic);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
        }
    }
}
