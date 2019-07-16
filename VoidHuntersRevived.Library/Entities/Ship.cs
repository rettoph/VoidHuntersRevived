using Guppy.Configurations;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.CustomEventArgs;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// A ship represents a player controlled in-game object.
    /// A player object can gain control of a ship at any time
    /// </summary>
    public partial class Ship : NetworkEntity
    {
        #region Public Attributes
        public TractorBeam TractorBeam { get; private set; }

        public ShipPart Bridge { get; private set; }

        public Player Player { get; internal set; }
        #endregion

        #region Events
        public event EventHandler<ChangedEventArgs<ShipPart>> OnBridgeChanged;
        public event EventHandler<ChangedEventArgs<Player>> OnPlayerChanged;
        #endregion

        #region Constructors
        public Ship(EntityConfiguration configuration, IServiceProvider provider) : base(configuration, provider)
        {
        }
        public Ship(Guid id, EntityConfiguration configuration, IServiceProvider provider) : base(id, configuration, provider)
        {
        }
        #endregion

        #region Initialization Methods
        protected override void Boot()
        {
            base.Boot();

            this.SetUpdateOrder(91);

            this.TractorBeam = this.entities.Create<TractorBeam>("entity:tractor-beam");
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Initialize any partial components...
            this.InitializeMovement();
        }
        #endregion

        #region Frame Methods
        protected override void update(GameTime gameTime)
        {
            base.update(gameTime);

            // Call component update methods...
            this.UpdateMovement(gameTime);
        }
        #endregion

        #region Setter Methods
        /// <summary>
        /// Attempt to update the current ship's bridge.
        /// If the bridge is successfully updated the 
        /// OnBridgeChanged event will be triggered.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Boolean TrySetBridge(ShipPart target)
        {
            if(target != this.Bridge && target.IsRoot && !target.IsBridge && !target.Focused)
            {
                this.logger.LogDebug($"Setting Ship({this.Id}) bridge to ShipPart<{target.GetType().Name}>({target.Id})...");

                var old = this.Bridge;

                this.Bridge = target;
                target.BridgeFor = this;

                // Trigger the bridge changed event
                this.OnBridgeChanged?.Invoke(this, new ChangedEventArgs<ShipPart>(old, target));
            }

            return false;
        }

        internal void SetPlayer(Player player)
        {
            var old = this.Player;
            this.Player = player;

            this.OnPlayerChanged?.Invoke(this, new ChangedEventArgs<Player>(old, this.Player));
        }
        #endregion

        #region Network Methods
        protected override void read(NetIncomingMessage im)
        {
            this.ReadBridgeData(im);
        }

        protected override void write(NetOutgoingMessage om)
        {
            this.WriteBridgeData(om);
        }

        /// <summary>
        /// Extract the bridge data from in incoming network message
        /// and attempt to update the current ship's bridge value
        /// </summary>
        /// <param name="im"></param>
        public void ReadBridgeData(NetIncomingMessage im)
        {
            // Update the local bridge value
            this.TrySetBridge(
                im.ReadEntity<ShipPart>(this.entities));
        }

        /// <summary>
        /// Add the bridge data to an outgoing network message
        /// </summary>
        /// <param name="om"></param>
        public void WriteBridgeData(NetOutgoingMessage om)
        {
            om.Write(this.Bridge);
        }
        #endregion
    }
}
