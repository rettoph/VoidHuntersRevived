using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using GalacticFighters.Client.Library.Utilities;
using GalacticFighters.Library.Entities.ShipParts;
using GalacticFighters.Library.Entities.ShipParts.Weapons;
using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Drivers.Entities.ShipParts.Weapons
{
    [IsDriver(typeof(Weapon))]
    internal sealed class ClientWeaponDriver : Driver<Weapon>
    {
        #region Private Fields
        private Body _serverBarrel;
        private ServerRender _server;
        private RevoluteJoint _joint;
        private Body _serverRoot;
        #endregion

        #region Constructor
        public ClientWeaponDriver(ServerRender server, Weapon driven) : base(driven)
        {
            _server = server;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            _serverBarrel = this.driven.Barrel.DeepClone(_server.World);

            this.driven.Events.TryAdd<ShipPart.ChainUpdate>("chain:updated", this.HandleChainUpdated);
            this.driven.Events.TryAdd<NetIncomingMessage>("read", this.HandleRead);
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
            if (this.driven.Root.Reserverd.Value)
                this.driven.UpdateBarrelPosition(_serverRoot, _serverBarrel);

            this.driven.UpdateBarrelAngle(_joint, _serverRoot);
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
            _serverBarrel.CollidesWith = this.driven.Root.CollidesWith;
            _serverBarrel.CollisionCategories = this.driven.Root.CollisionCategories;

            // Update the server render weld joint
            _serverRoot = _server.GetBodyById(this.driven.Root.BodyId);
            this.driven.UpdateJoint(ref _joint, _serverRoot, _serverBarrel, _server.World);
        }

        private void HandleRead(object sender, NetIncomingMessage arg)
        {
            this.driven.UpdateBarrelPosition(_serverRoot, _serverBarrel);

            this.driven.UpdateBarrelAngle(_joint, _serverRoot);
        }
        #endregion
    }
}
