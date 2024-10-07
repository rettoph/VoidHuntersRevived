using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common.Attributes;
using Guppy.Core.Network.Common.Enums;
using Guppy.Game.Graphics.Common;
using Guppy.Game.Input.Common;
using Guppy.Game.Input.Common.Messages;
using Guppy.Game.MonoGame.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [StrategyFilter(StrategyTypeEnum.Lockstep)]
    [PeerFilter(PeerType.Client)]
    internal sealed class CameraEngine : StrategyEngine, IOnDrawEngine,
        IInputSubscriber<CursorScroll>
    {
        private readonly ICamera2D _camera;
        private readonly IScreen _screen;
        private Vector2 _offset;
        private readonly IUserShipService _userShipService;
        private readonly IEntityQueryService _entitieQueryService;

        public CameraEngine(
            ICamera2D camera,
            IScreen screen,
            IUserShipService userShipService,
            IEntityQueryService entityQueryService)
        {
            _camera = camera;
            _camera.Zoom = 100;

            _screen = screen;
            _userShipService = userShipService;
            _entitieQueryService = entityQueryService;
        }


        [SequenceGroup<OnDrawSequenceGroup>(OnDrawSequenceGroup.PreDraw)]
        public void OnDraw(GameTime gameTime)
        {
            _screen.Camera.Update(gameTime);
            _camera.Update(gameTime);

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

            Vector2 location = Vector2.Zero;
            if (_userShipService.TryGetCurrentUserShipId(out EntityId shipId))
            {
                location = _entitieQueryService.QueryById<Location>(shipId).Position.ToXna();
            }

            _camera.Position = location + _offset;
        }

        public void Process(in Guid messageId, CursorScroll message)
        {
            _camera.Zoom *= ((float)Math.Pow(1.5, message.Delta / 120));
        }
    }
}
