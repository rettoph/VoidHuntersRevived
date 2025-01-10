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
        private readonly IEntityQueryService _entityQueryService;
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
            this._camera = camera;
            this._camera.Zoom = 100;
            this._zoom = 100;

            this._screen = screen;
            this._entityQueryService = entityQueryService;
            this._netScope = netScope;
        }


        [SequenceGroup<OnDrawSequenceGroupEnum>(OnDrawSequenceGroupEnum.PreDraw)]
        public void OnDraw(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                this._offset -= Vector2.UnitY * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                this._offset += Vector2.UnitY * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                this._offset -= Vector2.UnitX * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                this._offset += Vector2.UnitX * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            Vector2 location = this.GetUserPosition();
            this._position = location + this._offset;

            this._camera.Position = Vector2.Lerp(this._camera.Position, this._position, (float)gameTime.ElapsedGameTime.TotalSeconds * 2);
            this._camera.Zoom = MathHelper.Lerp(this._camera.Zoom, this._zoom, (float)gameTime.ElapsedGameTime.TotalSeconds * 2);


            this._screen.Camera.Update(gameTime);
            this._camera.Update(gameTime);
        }

        public void Process(in Guid messageId, CursorScroll message)
        {
            this._zoom *= ((float)Math.Pow(1.5, message.Delta / 120));
        }

        private Vector2 GetUserPosition()
        {
            Vector2 sum = Vector2.Zero;
            int count = 0;

            int currentUserId = this._netScope.Group.Peer.Users.Current.Id;
            ref var filter = ref this._entityQueryService.GetFilter<EntityLocalId, IUser>(currentUserId);
            foreach (var (indices, group) in filter)
            {
                var (bodies, _) = this._entityQueryService.QueryEntities<Body>(group);

                for (int i = 0; i < indices.count; i++)
                {
                    Body body = bodies[indices[i]];

                    count++;
                    sum += body.Transform.Position.ToXna();
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