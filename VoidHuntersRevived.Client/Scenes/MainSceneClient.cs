using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Client.Entities;
using VoidHuntersRevived.Core.Interfaces;
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
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _grapihcs.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
