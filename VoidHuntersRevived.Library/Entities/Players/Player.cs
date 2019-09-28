using GalacticFighters.Library.Scenes;
using Guppy;
using Guppy.Collections;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Interfaces;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Entities.Players
{
    /// <summary>
    /// Players represent objects that can preform actions within ships.
    /// A player will always contain a ship
    /// </summary>
    public abstract class Player : NetworkEntity
    {
        #region Private Fields

        #endregion

        #region Protected Attributes
        protected GalacticFightersWorldScene scene { get; private set; }
        #endregion

        #region Public Attributes
        public abstract String Name { get; }
        public Ship Ship { get; set; }
        #endregion

        #region Constructor
        public Player(GalacticFightersWorldScene scene)
        {
            this.scene = scene;
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            // Automatically add the current player instance to the scenes player collection.
            this.scene.Players.Add(this);

            this.SetUpdateOrder(200);
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
                this.Ship = this.entities.GetById<Ship>(im.ReadGuid());
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
