using Guppy.CommandLine.Services;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Extensions.System;
using Guppy.Network;
using Guppy.Network.Enums;
using Guppy.Network.Structs;
using Guppy.Threading.Interfaces;
using Guppy.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tainicom.Aether.Physics2D.Diagnostics;
using VoidHuntersRevived.Client.Library.Messages.Commands;
using VoidHuntersRevived.Library.Entities.Aether;

namespace VoidHuntersRevived.Client.Library.Entities.Aether
{
    public class AetherDebugView : BaseAetherWrapper<DebugView>,
        IDataProcessor<ToggleAetherDebugViewCommand>
    {
        private AetherWorld _world;
        private GraphicsDevice _graphics;
        private ContentManager _content;
        private Camera2D _camera;
        private SpriteFont _font;
        private Peer _peer;
        private SpriteBatch _spriteBatch;
        private CommandService _commands;
        private Boolean _visible;

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _font = provider.GetContent<SpriteFont>(Guppy.Constants.Content.DebugFont);


            provider.Service(out _world);
            provider.Service(out _graphics);
            provider.Service(out _content);
            provider.Service(out _camera);
            provider.Service(out _peer);
            provider.Service(out _spriteBatch);
            provider.Service(out _commands);

            this.BuildAetherInstances(provider);

            _commands.RegisterProcessor<ToggleAetherDebugViewCommand>(this);

            this.Visible = false;
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            _commands.DeregisterProcessor<ToggleAetherDebugViewCommand>(this);
        }

        protected override DebugView BuildInstance(ServiceProvider provider, NetworkAuthorization authorization)
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

        bool IDataProcessor<ToggleAetherDebugViewCommand>.Process(ToggleAetherDebugViewCommand data)
        {
            this.Visible = !this.Visible;

            return true;
        }
    }
}
