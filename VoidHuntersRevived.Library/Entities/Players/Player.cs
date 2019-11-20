using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.Players
{
    public abstract class Player : NetworkEntity
    {
        #region Public Properties
        public abstract String Name { get; }
        public Ship Ship { get; private set; }
        #endregion

        #region Setter Methods
        public void SetShip(Ship ship)
        {
            if(ship != this.Ship)
            { // Only update if the ship is different.
                this.Ship = ship;
            }
        }
        #endregion

        #region Network Methods
        protected override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            this.ReadShip(im);
        }

        protected override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            this.WriteShip(om);
        }

        /// <summary>
        /// Read & update the current player's ship data
        /// </summary>
        /// <param name="im"></param>
        public void ReadShip(NetIncomingMessage im)
        {
            if (im.ReadBoolean())
                this.SetShip(this.entities.GetById<Ship>(im.ReadGuid()));
        }

        /// <summary>
        /// Write the current player's ship data
        /// </summary>
        /// <param name="om"></param>
        public void WriteShip(NetOutgoingMessage om)
        {
            if (om.WriteExists(this.Ship))
                om.Write(this.Ship.Id);
        }
        #endregion
    }
}
