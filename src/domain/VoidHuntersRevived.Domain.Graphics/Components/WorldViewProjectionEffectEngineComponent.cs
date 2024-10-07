using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common;
using Guppy.Engine.Common.Components;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common.Components;
using Guppy.Game.Common.Enums;
using Guppy.Game.MonoGame.Common.Utilities.Cameras;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Graphics.Common.Effects;

namespace VoidHuntersRevived.Domain.Graphics.Components
{
    [AutoLoad]
    public class WorldViewProjectionEffectEngineComponent(
        Camera2D camera,
        IEnumerable<IWorldViewProjectionEffect> effects
    ) : IEngineComponent, IDrawableComponent
    {
        private readonly Camera2D _camera = camera;
        private readonly IWorldViewProjectionEffect[] _effects = [.. effects];

        [SequenceGroup<InitializeComponentSequenceGroup>(InitializeComponentSequenceGroup.Initialize)]
        public void Initialize(IGuppyEngine engine)
        {
            //
        }

        [SequenceGroup<DrawComponentSequenceGroup>(DrawComponentSequenceGroup.PreDraw)]
        public void Draw(GameTime gameTime)
        {
            Matrix worldViewprojection = _camera.World * _camera.View * _camera.Projection;

            foreach (IWorldViewProjectionEffect effect in _effects)
            {
                effect.WorldViewProjection = worldViewprojection;
            }
        }
    }
}
