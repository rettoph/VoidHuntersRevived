using Guppy.MonoGame.Utilities.Cameras;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Systems;

namespace VoidHuntersRevived.Library.Client.Systems
{
    internal sealed class CameraSystem : DrawSystem
    {
        private readonly Camera2D _camera;

        public CameraSystem(Camera2D camera)
        {
            _camera = camera;

            _camera.Zoom = 100;
        }

        public override void Draw(GameTime gameTime)
        {
            _camera.Update(gameTime);
        }
    }
}
