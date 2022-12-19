using Guppy.Attributes;
using Guppy.Common;
using Guppy.MonoGame;
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
using VoidHuntersRevived.Client.Library.Services;
using VoidHuntersRevived.Library;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Client.Library.Systems
{
#if DEBUG
    [AutoSubscribe]
#endif
    [AutoLoad]
    [GuppyFilter(typeof(GameGuppy))]
    internal sealed class AetherDebugSystem : DrawSystem, ISubscriber<Tick>, ISubscriber<BodyPosition>
    {
        private readonly AetherWorld _aether;
        private readonly DebugView _debug;
        private readonly GraphicsDevice _graphics;
        private readonly ContentManager _content;
        private readonly Camera2D _camera;
        private readonly PrimitiveBatch<VertexPositionColor> _primitiveBatch;
        private readonly Lazy<AetherBodyPositionDebugService> _bodyPositionDebugService;

        public AetherDebugSystem(
            Camera2D camera, 
            AetherWorld aether, 
            GraphicsDevice graphics, 
            ContentManager content,
            PrimitiveBatch<VertexPositionColor> primitiveBatch,
            Lazy<AetherBodyPositionDebugService> bodyPositionDebugService)
        {
            _camera = camera;
            _aether = aether;
            _graphics = graphics;
            _content = content;
            _primitiveBatch = primitiveBatch;
            _bodyPositionDebugService = bodyPositionDebugService;

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

#if DEBUG
            _primitiveBatch.Begin(_camera);
            _bodyPositionDebugService.Value.Draw(_primitiveBatch);
            _primitiveBatch.End();
#endif
        }

        public void Process(in Tick message)
        {
            foreach(var body in _aether.BodyList)
            {
                _bodyPositionDebugService.Value.AddPosition(true, body.GetHashCode(), body.Position);
            }
        }

        public void Process(in BodyPosition message)
        {
            _bodyPositionDebugService.Value.AddPosition(false, message.Id, message.Position);
        }
    }
}
