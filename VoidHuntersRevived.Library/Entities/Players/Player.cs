using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Entities.Players
{
    public abstract class Player : NetworkEntity
    {
        #region Protected Properties
        protected List<Player> players { get; private set; }
        #endregion

        #region Public Properties
        public abstract String Name { get; }
        public Ship Ship { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            this.players = provider.GetRequiredService<List<Player>>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.players.Add(this);
        }

        public override void Dispose()
        {
            base.Dispose();

            this.players.Remove(this);
        }
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
