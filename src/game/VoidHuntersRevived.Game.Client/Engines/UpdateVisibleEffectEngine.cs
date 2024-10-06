using Guppy.Core.Common.Attributes;
using Guppy.Game.MonoGame.Common.Utilities.Cameras;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Game.Client.Graphics.Effects;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [StrategyFilter(StrategyTypeEnum.Predictive)]
    public class UpdateVisibleEffectEngine(Camera2D camera, VisibleEffect effect) : StrategyEngine, IOnDrawEngine
    {
        private readonly Camera2D _camera = camera;
        private readonly VisibleEffect _effect = effect;

        [SequenceGroup<OnDrawSequenceGroup>(OnDrawSequenceGroup.PreDraw)]
        public void OnDraw(GameTime gameTime)
        {
            _effect.WorldViewProjection = _camera.World * _camera.View * _camera.Projection;
        }
    }
}
