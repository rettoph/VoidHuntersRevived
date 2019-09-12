using GalacticFighters.Library.Entities.ShipParts;
using Guppy.Collections;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Entities
{
    /// <summary>
    /// A ship represents a specific chain of pieces
    /// that can be controlled.
    /// </summary>
    public class Ship : NetworkEntity
    {
        #region Public Attributes
        public ShipPart Bridge { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            this.Events.Register<ShipPart>("bridge:changed");
        }
        #endregion

        #region Set Methods
        public void SetBridge(ShipPart bridge)
        {
            if(this.Bridge != bridge)
            {
                this.Bridge = bridge;

                this.Events.TryInvoke<ShipPart>(this, "bridge:changed", this.Bridge);
            }
        }
        #endregion

        #region Network Methods
        protected override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            this.WriteBridge(om);
        }

        protected override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            this.ReadBridge(im);
        }

        /// <summary>
        /// Write the Ship's current bridge data
        /// </summary>
        /// <param name="om"></param>
        public void WriteBridge(NetOutgoingMessage om)
        {
            if (om.WriteExists(this.Bridge))
                om.Write(this.Bridge.Id);
        }

        /// <summary>
        /// Read & update the current bridge data
        /// </summary>
        /// <param name="im"></param>
        public void ReadBridge(NetIncomingMessage im)
        {
            if (im.ReadBoolean())
                this.SetBridge(this.entities.GetById<ShipPart>(im.ReadGuid()));
        }
        #endregion
    }
}
