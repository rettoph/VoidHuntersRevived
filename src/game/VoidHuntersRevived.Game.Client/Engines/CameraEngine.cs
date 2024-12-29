using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common;
using Guppy.Game.Graphics.Common;
using Guppy.Game.Input.Common;
using Guppy.Game.Input.Common.Messages;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Game.Client.Engines
{
    internal sealed class CameraEngine : StrategyEngine,
        IClientEngine,
        IOnDrawEngine,
        IInputSubscriber<CursorScroll>
    {
        private readonly ICamera2D _camera;
        private readonly IScreen _screen;
        private readonly IEntityQueryService _entitieQueryService;
        private readonly INetScope<IStrategy> _netScope;
        private Vector2 _offset;

        private Vector2 _position;
        private float _zoom;

        public CameraEngine(
            ICamera2D camera,
            IScreen screen,
            IEntityQueryService entityQueryService,
            INetScope<IStrategy> netScope)
        {
            _camera = camera;
            _camera.Zoom = 100;
            _zoom = 100;

            _screen = screen;
            _entitieQueryService = entityQueryService;
            _netScope = netScope;
        }


        [SequenceGroup<OnDrawSequenceGroup>(OnDrawSequenceGroup.PreDraw)]
        public void OnDraw(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                _offset -= Vector2.UnitY * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                _offset += Vector2.UnitY * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                _offset -= Vector2.UnitX * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _offset += Vector2.UnitX * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            Vector2 location = this.GetUserPosition();
            _position = location + _offset;

            _camera.Position = Vector2.Lerp(_camera.Position, _position, (float)gameTime.ElapsedGameTime.TotalSeconds * 2);
            _camera.Zoom = MathHelper.Lerp(_camera.Zoom, _zoom, (float)gameTime.ElapsedGameTime.TotalSeconds * 2);


            _screen.Camera.Update(gameTime);
            _camera.Update(gameTime);
        }

        public void Process(in Guid messageId, CursorScroll message)
        {
            _zoom *= ((float)Math.Pow(1.5, message.Delta / 120));
        }

        private Vector2 GetUserPosition()
        {
            Vector2 sum = Vector2.Zero;
            int count = 0;

            int currentUserId = _netScope.Group.Peer.Users.Current.Id;
            ref var filter = ref _entitieQueryService.GetFilter<EntityLocalId, IUser>(currentUserId);
            foreach (var (indices, group) in filter)
            {
                var (locations, _) = _entitieQueryService.QueryEntities<Body>(group);

                for (int i = 0; i < indices.count; i++)
                {
                    Body location = locations[indices[i]];

                    count++;
                    sum += location.Transform.Position.ToXna();
                }
            }

            if (count == 0)
            {
                return sum;
            }

            // Average the position of all user controlled entities
            return sum / count;
        }
    }
}
