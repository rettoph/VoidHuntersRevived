using Guppy.MonoGame;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Game.Client.GameComponents
{
    internal sealed class ScreenComponent : SimpleGameComponent
    {
        private readonly IScreen _screen;

        public ScreenComponent(IScreen screen)
        {
            _screen = screen;
        }

        public override void Update(GameTime gameTime)
        {
            _screen.Camera.Update(gameTime);
        }
    }
}
