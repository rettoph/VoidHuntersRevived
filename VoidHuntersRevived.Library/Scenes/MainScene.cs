﻿using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Library.Entities;

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

            // Create and setup a new wall
            var wall = this.Entities.Create<Wall>("entity:wall");
            wall.Configure(5, 5);

            for (Int32 i = 0; i < 5; i++)
            {
                var box = BodyFactory.CreateRectangle(
                    this.World,
                    0.5f,
                    0.5f,
                    10f,
                    new Vector2(0f, 0.5f),
                    0.785398f,
                    BodyType.Dynamic);

                box.LinearVelocity = new Vector2(1, -1);
                box.Restitution = 1f;
                box.Friction = 0f;

                box.FixtureList[0].Restitution = -1f;
                box.FixtureList[0].Friction = 0f;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000);
        }
    }
}
