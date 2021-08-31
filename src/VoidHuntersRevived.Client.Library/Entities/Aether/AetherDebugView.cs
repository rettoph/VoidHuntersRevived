using Guppy.DependencyInjection;
using Guppy.Extensions.System;
using Guppy.Network.Enums;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Diagnostics;
using VoidHuntersRevived.Library.Entities.Aether;

namespace VoidHuntersRevived.Client.Library.Entities.Aether
{
    public class AetherDebugView : BaseAetherWrapper<DebugView>
    {
        private AetherWorld _world;
        private GraphicsDevice _graphics;
        private ContentManager _content;
        private Camera2D _camera;

        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _world);
            provider.Service(out _graphics);
            provider.Service(out _content);
            provider.Service(out _camera);

            this.BuildAetherInstances(provider);
        }

        protected override DebugView BuildInstance(GuppyServiceProvider provider, NetworkAuthorization authorization)
        {
            return new DebugView(_world.Instances[authorization]).Then(debugView =>
            {
                debugView.LoadContent(_graphics, _content);

                if(authorization == NetworkAuthorization.Slave)
                {
                    debugView.InactiveShapeColor = Color.Green;
                    debugView.DefaultShapeColor = Color.Green;
                }
            });
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.Do(debugView => debugView.RenderDebugData(_camera.Projection, _camera.View));
        }
    }
}
