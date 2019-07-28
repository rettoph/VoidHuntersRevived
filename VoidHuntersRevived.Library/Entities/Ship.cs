using Guppy.Configurations;
using Guppy.Interfaces;
using Guppy.Network;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.CustomEventArgs;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// A ship represents a player controlled in-game object.
    /// A player object can gain control of a ship at any time
    /// </summary>
    public partial class Ship : NetworkEntity
    {
        #region Private Fields
        private List<ShipPart> _children;
        #endregion

        #region Protected Attributes
        protected List<ShipPart> children { get { return _children; } }
        #endregion

        #region Public Attributes
        public TractorBeam TractorBeam { get; private set; }

        public ShipPart Bridge { get; private set; }

        public Player Player { get; internal set; }
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

            _children = new List<ShipPart>();

            this.SetUpdateOrder(91);

            this.TractorBeam = this.entities.Create<TractorBeam>("entity:tractor-beam", this);
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

                // Hold onto the old bridge so we can trigger a changed event
                var old = this.Bridge;

                // Unbind the old bridge...
                if (this.Bridge != null)
                {
                    this.Bridge.BridgeFor = null;

                    this.Bridge.OnConnectionNodesRemapped -= this.HandleBridgeConnectionNodesRemapped;
                    this.Bridge.Disposing -= this.HandleBridgeDisposing;
                }

                // Setup the new bridge
                this.Bridge = target;
                if (this.Bridge != null)
                {
                    this.Bridge.BridgeFor = this;

                    this.Bridge.OnConnectionNodesRemapped += this.HandleBridgeConnectionNodesRemapped;
                    this.Bridge.Disposing += this.HandleBridgeDisposing;
                }

                //Remap the bridge
                this.RemapBridge();

                // Trigger the bridge changed event
                this.Events.TryInvoke("changed:bridge", this.Bridge);
            }

            return false;
        }

        internal void SetPlayer(Player player)
        {
            var old = this.Player;
            this.Player = player;

            this.Events.TryInvoke("changed:player", this.Player);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When the bridge ship-part is disposed of,
        /// we must remove the current bridge value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleBridgeDisposing(object sender, ITrackedDisposable e)
        {
            this.TrySetBridge(null);
        }

        /// <summary>
        /// When the bridge connection nodes are updated, we must remap all thrusters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleBridgeConnectionNodesRemapped(object sender, ShipPart e)
        {
            this.RemapBridge();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Used to remap the bridge ship parts, thrusters, weapons,
        /// and other special parts.
        /// </summary>
        protected void RemapBridge()
        {
            this.children.Clear();
            this.Bridge?.GetChildren(ref _children);

            this.RemapThrusters();
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

        public void ReadDirectionData(NetIncomingMessage im)
        {
            this.SetDirection(
                (Direction)im.ReadByte(),
                im.ReadBoolean());
        }

        public void WriteDirectionData(NetOutgoingMessage om, Direction direction)
        {
            om.Write((Byte)direction);
            om.Write(_directions[direction]);
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();

            // Unset the bridge...
            this.TrySetBridge(null);

            // Dispose of internal objects...
            this.TractorBeam.Dispose();
        }
    }
}
