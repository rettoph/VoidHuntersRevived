using Guppy.Configurations;
using Guppy.Network;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Network.Extensions.Lidgren;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Library.CustomEventArgs;

namespace VoidHuntersRevived.Library.Entities.Players
{
    /// <summary>
    /// The player class is used to interface directly with a ship.
    /// Currently, only user players exist but AI players could
    /// be developed using this class as a base in the future.
    /// </summary>
    public abstract class Player : NetworkEntity
    {
        #region Public Attributes
        public abstract String Name { get; }
        public Ship Ship { get; private set; }
        #endregion

        #region Constructors
        public Player(EntityConfiguration configuration, IServiceProvider provider) : base(configuration, provider)
        {
        }
        public Player(Guid id, EntityConfiguration configuration, IServiceProvider provider) : base(id, configuration, provider)
        {
        }
        #endregion

        #region Initialization Methods
        protected override void Boot()
        {
            base.Boot();

            this.SetUpdateOrder(90);
        }
        #endregion

        #region Set Methods
        public Boolean TrySetShip(Ship target)
        {
            if (target != this.Ship && target.Player == null)
            {
                this.logger.LogDebug($"Setting Player<{this.GetType().Name}>({this.Id}) ship to Ship({target.Id})...");

                this.Ship = target;
                this.Ship.SetPlayer(this);
                this.Ship.Dirty = true;

                this.Events.TryInvoke("changed:ship", this.Ship);
                return true;
            }

            return false;
        }
        #endregion

        #region Network Methods
        protected override void read(NetIncomingMessage im)
        {
            this.ReadShipData(im);
        }

        protected override void write(NetOutgoingMessage om)
        {
            this.WriteShipData(om);
        }

        public void ReadShipData(NetIncomingMessage im)
        {
            this.TrySetShip(
                im.ReadEntity<Ship>(this.entities));
        }

        public void WriteShipData(NetOutgoingMessage om)
        {
            om.Write(this.Ship);
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();

            // Dispose of the old ships player
            this.Ship.SetPlayer(null);
        }
    }
}
