using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Client.Entities;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Client.Scenes
{
    public class MainSceneClient : MainScene
    {
        public Camera Camera { get; set; }

        public MainSceneClient(IServiceProvider provider, IGame game) : base(provider, game)
        {
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
    }
}
