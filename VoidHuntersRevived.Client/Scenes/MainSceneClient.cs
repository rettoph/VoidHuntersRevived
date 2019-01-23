using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Client.Entities;
using VoidHuntersRevived.Client.Entities.Ships;
using VoidHuntersRevived.Client.Layers;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Scenes
{
    public class MainSceneClient : MainScene
    {
        public Camera Camera { get; set; }
        public Cursor Cursor { get; set; }
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

            var layer = this.Layers.Create<FarseerEntityLayer>();

            // Create the basic global entities
            this.Cursor = this.Entities.Create<Cursor>("entity:cursor");
            this.Camera = this.Entities.Create<Camera>("entity:camera");

            var ship = this.Entities.Create<CurrentClientShip>("entity:ship:current_client");
            

            // Create a new test centerpiece
            var center = this.Entities.Create<Hull>("entity:hull_square", layer);
            center.Body.Mass = 10f;
            this.Camera.Follow = center;

            // Generate a random map of parts
            var random = new Random(1);

            for (var i = 0; i < 20; i++) {
                var entity = this.Entities.Create<Hull>("entity:hull_square", layer);
                entity.Body.Position = new Vector2(
                    (float)(random.NextDouble() * this.Wall.Boundaries.Width) + this.Wall.Boundaries.Left,
                    (float)(random.NextDouble() * this.Wall.Boundaries.Height) + this.Wall.Boundaries.Top);
                entity.Body.Rotation = (float)random.NextDouble() * (float)Math.PI;
            }

            for (var i = 0; i < 20; i++)
            {
                var entity = this.Entities.Create<Hull>("entity:hull_beam", layer);
                entity.Body.Position = new Vector2(
                    (float)(random.NextDouble() * this.Wall.Boundaries.Width) + this.Wall.Boundaries.Left,
                    (float)(random.NextDouble() * this.Wall.Boundaries.Height) + this.Wall.Boundaries.Top);
                entity.Body.Rotation = (float)random.NextDouble() * (float)Math.PI;
            }

            
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
