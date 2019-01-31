﻿using FarseerPhysics.Dynamics;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Networking.Implementations;

namespace VoidHuntersRevived.Library.Entities.Players
{
    public abstract class Player : NetworkEntity, IPlayer
    {
        public abstract String Name { get; }
        public TractorBeam TractorBeam { get; private set; }
        public Hull Bridge { get; private set; }
        public Boolean[] Movement { get; set; }

        public Player(Hull bridge, EntityInfo info, IGame game) : base(info, game)
        {
            this.SetBridge(bridge);

            this.Enabled = true;
        }

        public Player(long id, EntityInfo info, IGame game) : base(id, info, game)
        {
            this.Enabled = true;
        }

        #region Initialization Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Set the default movement values
            this.Movement = new Boolean[]
            {
                false,
                false,
                false,
                false,
            };
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Create a new tractorbeam for the player
            this.TractorBeam = this.Scene.Entities.Create<TractorBeam>("entity:tractor_beam", null, this);
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Move the bridge relative to the requested movement values
            if (this.Bridge != null)
            {
                if (this.Movement[0])
                    this.Bridge.Body.ApplyForce(Vector2.Transform(new Vector2(200, 0), this.Bridge.RotationMatrix));
                if (this.Movement[2])
                    this.Bridge.Body.ApplyForce(Vector2.Transform(new Vector2(-200, 0), this.Bridge.RotationMatrix));

                if (this.Movement[1])
                    this.Bridge.Body.ApplyAngularImpulse(0.001f);
                if (this.Movement[3])
                    this.Bridge.Body.ApplyAngularImpulse(-0.001f);
            }
        }



        /// <summary>
        /// Update the current players bridge, if possible
        /// </summary>
        /// <param name="bridge"></param>
        public virtual void SetBridge(Hull bridge)
        {
            this.Game.Logger.LogDebug($"Updating Bridge!");

            if (this.Bridge != null)
            { // reghost the old bridge
                this.Bridge.BridgeFor = null;
                this.Bridge.SetGhost(true);
                this.Bridge.SetEnabled(false);
            }

            this.Bridge = bridge;
            this.Bridge.BridgeFor = this;
            this.Bridge.SetGhost(false);
            this.Bridge.SetEnabled(true);            
        }

        #region Network Read & Write methods
        public override void Read(NetIncomingMessage im)
        {
            // Update the movement inputs
            this.Movement[0] = im.ReadBoolean();
            this.Movement[1] = im.ReadBoolean();
            this.Movement[2] = im.ReadBoolean();
            this.Movement[3] = im.ReadBoolean();

            // Update the tractor beam settings
            if(im.ReadBoolean())
                this.TractorBeam.Read(im);
        }

        public override void Write(NetOutgoingMessage om)
        {
            om.Write(this.Id);

            // Write the ship movements
            om.Write(this.Movement[0]);
            om.Write(this.Movement[1]);
            om.Write(this.Movement[2]);
            om.Write(this.Movement[3]);

            // Write the tractor beam settings
            if(this.TractorBeam == null)
            {
                om.Write(false);
            }
            else
            {
                om.Write(true);
                this.TractorBeam.Write(om);
            }
            
        }
        #endregion
    }
}
