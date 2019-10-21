using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Weapons;
using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using VoidHuntersRevived.Client.Library.Scenes;
using Guppy.Loaders;
using Guppy.Utilities.Cameras;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Utilities.Controllers;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.ShipParts.Weapons
{
    [IsDriver(typeof(Weapon), 200)]
    internal sealed class ClientWeaponDriver : Driver<Weapon>
    {
        #region Private Fields
        private Body _serverBarrel;
        private ServerRender _server;
        private RevoluteJoint _serverJoint;
        private Body _serverRoot;
        /// <summary>
        /// The stored root when the chain is updated.
        /// There are certain events that must be bound and unbound to
        /// each time a chain updates. This allows us to easily access
        /// the old root
        /// </summary>
        private ShipPart _root;

        private Camera _camera;
        private SpriteManager _sprite;
        #endregion

        #region Constructor
        public ClientWeaponDriver(SpriteManager sprite, ClientWorldScene scene, ServerRender server, Weapon driven) : base(driven)
        {
            _server = server;
            _sprite = sprite;
            _camera = scene.Camera;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            _serverBarrel = this.driven.Barrel.DeepClone(_server.World);

            this.driven.Events.TryAdd<ShipPart.ChainUpdate>("chain:updated", this.HandleChainUpdated);
            this.driven.Events.TryAdd<Vector2>("target:updated", this.HandleTargetUpdated);
            this.driven.Events.TryAdd<Controller>("controller:changed", this.HandleControllerChanged);

            var config = (driven.Configuration.Data as WeaponConfiguration);
            _sprite.Load(config.BarrelTexture, config.Barrel);
        }

        protected override void Dispose()
        {
            base.Dispose();

            _serverBarrel.Dispose();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // When reserved, instantly update server barrel position so the joint doesnt have to
            this.driven.UpdateBarrelPosition(_serverRoot, _serverBarrel);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            var fullColor = !(this.driven.Controller is Chunk) ? ColorScheme.Blue : (this.driven.Root.Configuration.Data as ShipPartConfiguration).DefaultColor;
            var deadColor = Color.Lerp(ColorScheme.Red, fullColor, 0.2f);

            _sprite.Draw(
                this.driven.WorldBodyAnchor, 
                this.driven.Rotation + this.driven.JointAngle + MathHelper.Pi,
                Color.Lerp(deadColor, fullColor, this.driven.Health / 100));
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When the client side chain recieves a chain update,
        /// we must ensure that the server render collision categories
        /// are updated as well
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void HandleChainUpdated(object sender, ShipPart.ChainUpdate arg)
        {
            // Update the server render weld joint
            _serverRoot = _server.GetBodyById(this.driven.Root.BodyId);

            // Update joint data
            this.driven.UpdateJoint(ref _serverJoint, _serverRoot, _serverBarrel, _server.World);

            // Update barrel info
            _serverBarrel.CollidesWith = this.driven.Root.CollidesWith;
            _serverBarrel.CollisionCategories = this.driven.Root.CollisionCategories;
            _serverBarrel.IgnoreCCDWith = this.driven.Root.IgnoreCCDWith;

            if (_root != default(ShipPart))
            { // Remove old events
                _root.Events.TryRemove<Boolean>("body-enabled:changed", this.HandleFarseerEnabledChanged);
                _root.Events.TryRemove<Body>("position:changed", this.HandlePositionChanged);
                _root.Events.TryRemove<NetIncomingMessage>("read", this.HandleRead);
            }

            // Save new events
            _root = this.driven.Root;
            _root.Events.TryAdd<Boolean>("body-enabled:changed", this.HandleFarseerEnabledChanged);
            _root.Events.TryAdd<Body>("position:changed", this.HandlePositionChanged);
            _root.Events.TryAdd<NetIncomingMessage>("read", this.HandleRead);
        }

        private void HandlePositionChanged(object sender, Body arg)
        {
            this.driven.UpdateBarrelPosition(_serverRoot, _serverBarrel);
        }

        private void HandleFarseerEnabledChanged(object sender, bool arg)
        {
            _serverBarrel.Enabled = arg;
        }

        private void HandleRead(object sender, NetIncomingMessage arg)
        {
            this.driven.UpdateBarrelPosition(_serverRoot, _serverBarrel);
            this.driven.UpdateBarrelTarget(this.driven.WorldBodyAnchor, _serverJoint, _serverRoot);
        }

        private void HandleTargetUpdated(object sender, Vector2 target)
        {
            this.driven.UpdateBarrelTarget(target, _serverJoint, _serverRoot);
        }

        private void HandleControllerChanged(object sender, Controller arg)
        {
            _serverBarrel.CollidesWith = this.driven.CollidesWith;
            _serverBarrel.CollisionCategories = this.driven.CollisionCategories;
            _serverBarrel.IgnoreCCDWith = this.driven.IgnoreCCDWith;
        }
        #endregion
    }
}
