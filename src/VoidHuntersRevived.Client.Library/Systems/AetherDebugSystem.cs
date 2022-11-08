using Guppy.Attributes;
using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Diagnostics;
using VoidHuntersRevived.Library;

namespace VoidHuntersRevived.Client.Library.Systems
{
    [AutoLoad]
    [GuppyFilter(typeof(GameGuppy))]
    internal sealed class AetherDebugSystem : DrawSystem
    {
        private readonly AetherWorld _aether;
        private readonly DebugView _debug;
        private readonly GraphicsDevice _graphics;
        private readonly ContentManager _content;
        private readonly Camera2D _camera;

        public AetherDebugSystem(Camera2D camera, AetherWorld aether, GraphicsDevice graphics, ContentManager content)
        {
            _camera = camera;
            _aether = aether;
            _graphics = graphics;
            _content = content;

            _debug = new DebugView(_aether);
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _debug.LoadContent(_graphics, _content);
            _camera.ZoomTo(100);
        }

        public override void Draw(GameTime gameTime)
        {
            _camera.Update(gameTime);

            _debug.RenderDebugData(_camera.Projection, _camera.View);
        }
    }
}
