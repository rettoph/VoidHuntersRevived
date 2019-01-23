using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VoidHuntersRevived.Client.Entities;
using VoidHuntersRevived.Client.Scenes;
using VoidHuntersRevived.Core.Implementations;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Client.Services
{
    public class CameraControllerService : SceneObject, ISceneService
    {
        private MainSceneClient _scene;
        private Camera _camera;
        private Int32 _lastScrollWheelValue;
        private Int32 _scrollWheelDelta;


        public CameraControllerService(IGame game) : base(game)
        {
            this.Visible = false;
            this.Enabled = true;
        }

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            // Update zoom levels
            var mouse = Mouse.GetState();
            _scrollWheelDelta = _lastScrollWheelValue - mouse.ScrollWheelValue;

            _camera.Zoom *= 1 + ((_scrollWheelDelta/120) * -0.1f);

            _lastScrollWheelValue = mouse.ScrollWheelValue;

            // Update followed ship
            var keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.W))
                _camera.Follow.Body.ApplyLinearImpulse(new Vector2(0, -5));
            if (keyboard.IsKeyDown(Keys.A))
                _camera.Follow.Body.ApplyLinearImpulse(new Vector2(-5, 0));
            if (keyboard.IsKeyDown(Keys.S))
                _camera.Follow.Body.ApplyLinearImpulse(new Vector2(0, 5));
            if (keyboard.IsKeyDown(Keys.D))
                _camera.Follow.Body.ApplyLinearImpulse(new Vector2(5, 0));

            if (keyboard.IsKeyDown(Keys.Q))
                _camera.Follow.Body.ApplyAngularImpulse(-0.01f);

            if (keyboard.IsKeyDown(Keys.E))
                _camera.Follow.Body.ApplyAngularImpulse(0.01f);
        }

        protected override void Boot()
        {
            // throw new NotImplementedException();
        }

        protected override void PreInitialize()
        {
            // throw new NotImplementedException();
        }

        protected override void Initialize()
        {
            // throw new NotImplementedException();
        }

        protected override void PostInitialize()
        {
            _scene = this.Scene as MainSceneClient;
            _camera = _scene.Camera;
        }
    }
}
