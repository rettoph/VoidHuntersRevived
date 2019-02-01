using FarseerPhysics.Dynamics;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Core.Interfaces;
using VoidHuntersRevived.Core.Structs;
using VoidHuntersRevived.Library.Entities.Connections.Nodes;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Networking.Implementations;

namespace VoidHuntersRevived.Library.Entities.Players
{
    public abstract class Player : NetworkEntity, IPlayer
    {
        public abstract String Name { get; }
        public TractorBeam TractorBeam { get; private set; }
        public ShipPart Bridge { get; private set; }
        public Boolean[] Movement { get; set; }

        /// <summary>
        /// A list of all available female connection nodes found within the current bridge chain
        /// </summary>
        public FemaleConnectionNode[] AvailableFemaleConnectionNodes { get; private set; }

        public Player(ShipPart bridge, EntityInfo info, IGame game) : base(info, game)
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
                    this.Bridge.Body.ApplyForce(Vector2.Transform(new Vector2(1000, 0), this.Bridge.TransformationOffsetMatrix));
                if (this.Movement[2])
                    this.Bridge.Body.ApplyForce(Vector2.Transform(new Vector2(-1000, 0), this.Bridge.TransformationOffsetMatrix));

                if (this.Movement[1])
                    this.Bridge.Body.ApplyAngularImpulse(2f);
                if (this.Movement[3])
                    this.Bridge.Body.ApplyAngularImpulse(-2f);
            }
        }



        /// <summary>
        /// Update the current players bridge, if possible
        /// </summary>
        /// <param name="bridge"></param>
        public virtual void SetBridge(ShipPart bridge)
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

            this.UpdateAvailableFemaleConnectionNodes();
        }

        /// <summary>
        /// Dynamically search through the bridge connection chain
        /// and create ann array containing all available female connection
        /// nodes. Save that array to the local AvailableFemaleConnectionNodes array
        /// </summary>
        public void UpdateAvailableFemaleConnectionNodes()
        {
            this.AvailableFemaleConnectionNodes = this.Bridge.GetAvailabaleFemaleConnectioNodes().ToArray();
        }

        #region Network Read & Write methods
        public override void Read(NetIncomingMessage im)
        {
            // Update the movement inputs
            this.Movement[0] = im.ReadBoolean();
            this.Movement[1] = im.ReadBoolean();
            this.Movement[2] = im.ReadBoolean();
            this.Movement[3] = im.ReadBoolean();
        }

        public override void Write(NetOutgoingMessage om)
        {
            om.Write(this.Id);

            // Write the ship movements
            om.Write(this.Movement[0]);
            om.Write(this.Movement[1]);
            om.Write(this.Movement[2]);
            om.Write(this.Movement[3]);
        }
        #endregion
    }
}
