using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common.Attributes;
using Guppy.Core.Network.Common.Enums;
using Guppy.Game.Common.Enums;
using Guppy.Game.Input.Common;
using Guppy.Game.Input.Common.Messages;
using Guppy.Game.MonoGame.Common;
using Guppy.Game.MonoGame.Common.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Svelto.ECS;
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
    [Sequence<DrawSequence>(DrawSequence.PreDraw)]
    [PeerFilter(PeerType.Client)]
    internal sealed class CameraEngine : StrategyEngine, IStepEngine<GameTime>,
        IInputSubscriber<CursorScroll>
    {
        private readonly Camera2D _camera;
        private readonly IScreen _screen;
        private Vector2 _offset;
        private readonly IUserShipService _userShips;
        private readonly IEntityService _entities;

        public CameraEngine(IScreen screen, Camera2D camera, IUserShipService userShips, IEntityService entities)
        {
            _screen = screen;
            _camera = camera;
            _camera.Zoom = 100;
            _userShips = userShips;
            _entities = entities;
        }

        public string name { get; } = nameof(CameraEngine);

        public void Step(in GameTime _param)
        {
            _screen.Camera.Update(_param);
            _camera.Update(_param);

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                _offset -= Vector2.UnitY * (float)_param.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                _offset += Vector2.UnitY * (float)_param.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                _offset -= Vector2.UnitX * (float)_param.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _offset += Vector2.UnitX * (float)_param.ElapsedGameTime.TotalSeconds;
            }

            Vector2 location = Vector2.Zero;
            if (_userShips.TryGetCurrentUserShipId(out EntityId shipId))
            {
                location = _entities.QueryById<Location>(shipId).Position.ToXna();
            }

            _camera.TargetPosition = location + _offset;
        }

        public void Process(in Guid messageId, CursorScroll message)
        {
            _camera.TargetZoom *= ((float)Math.Pow(1.5, message.Delta / 120));
        }
    }
}
