using Guppy;
using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using VoidHuntersRevived.Library.Entities;
using Guppy.Extensions.Microsoft.Xna.Framework;
using Guppy.IO.Services;
using VoidHuntersRevived.Library.Scenes;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Extensions.Aether;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using Guppy.Utilities.Cameras;
using VoidHuntersRevived.Library;

namespace VoidHuntersRevived.Client.Library.Entities
{
    /// <summary>
    /// Simple entity used to interact with the farseer world.
    /// </summary>
    public class Sensor : Entity
    {
        #region Private Fields
        private MouseService _mouse;
        private WorldEntity _world;
        private Camera2D _camera;
        private Body _body;
        private HashSet<BodyEntity> _contacts;
        private GameScene _scene;
        #endregion

        #region Public Attributes
        public IEnumerable<BodyEntity> Contacts => _contacts;
        public Vector2 Position => _body.WorldCenter;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _mouse);
            provider.Service(out _camera);
            provider.Service(out _scene);

            _contacts = new HashSet<BodyEntity>();

            _scene.IfOrOnWorld(this.ConfigureAether);

            this.LayerGroup = VHR.LayersContexts.World.Group.GetValue();
        }

        protected override void Release()
        {
            base.Release();

            _body?.TryRemove();

            this.OnUpdate -= this.UpdateBody;

            _mouse = null;
            _camera = null;
            _scene = null;
        }
        #endregion

        #region Frame Methods
        private void UpdateBody(GameTime gameTime)
        {
            var position = _camera.Unproject(_mouse.Position.ToVector3());
            _body.SetTransform(position.ToVector2(), 0);
        }
        #endregion

        #region Event Handlers
        private void ConfigureAether(WorldEntity world)
        {
            _world = world;

            _body = _world.Live.CreateCircle(3f, 0f);
            _body.SetIsSensor(true);
            _body.SleepingAllowed = false;
            _body.BodyType = BodyType.Dynamic;

            _body.OnCollision += this.HandleSensorCollision;
            _body.OnSeparation += this.HandleSensorSeparation;

            this.OnUpdate += this.UpdateBody;
        }

        private bool HandleSensorCollision(Fixture sender, Fixture other, Contact contact)
        {
            if (sender.Tag is BodyEntity be1)
                _contacts.Add(be1);
            if (other.Tag is BodyEntity be2)
                _contacts.Add(be2);

            return true;
        }

        private void HandleSensorSeparation(Fixture sender, Fixture other, Contact contact)
        {
            if (sender.Tag is BodyEntity be1)
                _contacts.Remove(be1);
            if (other.Tag is BodyEntity be2)
                _contacts.Remove(be2);
        }
        #endregion
    }
}
