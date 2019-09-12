using GalacticFighters.Library.Configurations;
using GalacticFighters.Library.Extensions.Farseer;
using GalacticFighters.Library.Utilities;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Entities.ShipParts
{
    /// <summary>
    /// Contains miscellaneous code not specific to any other
    /// partial classes.
    /// </summary>
    public abstract partial class ShipPart
    {
        #region Private Fields
        private Boolean _wasAwake;
        #endregion

        #region Protected Fields
        protected ShipPartConfiguration config;
        #endregion

        #region Public Attributes
        /// <summary>
        /// Reservations can be made onto ShipParts.
        /// A shipPart with no reservations will automatically
        /// disable itself when the body falls asleep. If the ShipPart
        /// has a reservation, however, it will keep itself enabled.
        /// </summary>
        public CounterBoolean Reserved { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            // Automatically enable the ShipPart when a reservation is made.
            this.Reserved = new CounterBoolean(value => this.SetEnabled(true));

            // Call internal create functions
            this.ConnectionNode_Create(provider);
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Save the configuration
            this.config = this.Configuration.Data as ShipPartConfiguration;

            // Call internal pre initialize functions
            this.ConnectionNode_PreInitialize();
            this.Transformations_PreInitialize();
            this.Farseer_PreInitialize();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();

            this.body.OnCollision += (fa, fb, c) =>
            { // Automatically re-enable the current ship-part when there is a collision
                this.SetEnabled(true);
                return true;
            };
        }

        public override void Dispose()
        {
            base.Dispose();

            // Call internal dispose functions
            this.ConnectionNode_Dispose();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!this.Reserved.Value && !this.Awake && _wasAwake)
                this.SetEnabled(false);

            _wasAwake = this.Awake;
        }
        #endregion

        #region Network Methods
        protected override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            // Read vital data
            this.body.ReadPosition(im);
            this.body.ReadVelocity(im);

            this.ConnectionNode_Read(im);
        }

        protected override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            // Write vital data
            this.body.WritePosition(om);
            this.body.WriteVelocity(om);

            this.ConnectionNode_Write(om);
        }
        #endregion
    }
}
