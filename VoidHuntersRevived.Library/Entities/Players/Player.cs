using Guppy.Configurations;
using Guppy.Network;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Network.Extensions.Lidgren;
using Microsoft.Extensions.Logging;

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

        #region Events
        public event EventHandler<ChangedEventArgs<Ship>> OnShipChanged;
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

                var oldShip = this.Ship;
                this.Ship = target;
                this.Ship.SetPlayer(this);

                this.OnShipChanged?.Invoke(this, new ChangedEventArgs<Ship>(oldShip, this.Ship));

                return true;
            }

            return false;
        }
        #endregion

        #region Network Methods
        protected override void read(NetIncomingMessage im)
        {
            if(im.ReadBoolean())
            {
                this.TrySetShip(
                    this.entities.GetById<Ship>(
                        im.ReadGuid()));
            }
        }

        protected override void write(NetOutgoingMessage om)
        {
            if(this.Ship == null)
            {
                om.Write(false);
            }
            else
            {
                om.Write(true);
                om.Write(this.Ship.Id);
            }
            
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
