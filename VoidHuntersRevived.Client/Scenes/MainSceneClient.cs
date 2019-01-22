using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Client.Entities;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Scenes
{
    public class MainSceneClient : MainScene
    {
        public Camera Camera { get; set; }
        private GraphicsDevice _grapihcs;

        public MainSceneClient(GraphicsDevice graphics, IServiceProvider provider, IGame game) : base(provider, game)
        {
            _grapihcs = graphics;

            this.Visible = true;
            this.Enabled = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Create a new camera
            this.Camera = this.Entities.Create<Camera>("entity:camera");

            var center = this.Entities.Create<Hull>("entity:hull_square");

            var random = new Random(1);

            for (var i = 0; i < 100; i++) {
                var entity = this.Entities.Create<Hull>("entity:hull_square");
                entity.Body.Position = new Vector2((float)(random.NextDouble() * 149.9) - 74.95f, (float)(random.NextDouble() * 149.9) - 74.95f);
                entity.Body.Rotation = (float)random.NextDouble() * (float)Math.PI;
            }

            this.Camera.Follow = center;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _grapihcs.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}
