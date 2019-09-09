using GalacticFighters.Library.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Library.Entities.ShipParts
{
    /// <summary>
    /// Contains miscellaneous code not specific to any other
    /// partial classes.
    /// </summary>
    public partial class ShipPart
    {
        #region Private Fields
        private ShipPartConfiguration _configuration;
        #endregion

        #region Public Attributes
        /// <summary>
        /// The current ShipPart's immediate parent, if any.
        /// </summary>
        public ShipPart Parent { get { return this.MaleConnectionNode.Target?.Parent; } }

        /// <summary>
        /// Wether or not the current ShipPart is 
        /// </summary>
        public Boolean IsRoot { get { return !this.MaleConnectionNode.Connected; } }

        /// <summary>
        /// Return the root most ShipPart in the current ShipPart's chain.
        /// </summary>
        public ShipPart Root { get { return this.IsRoot ? this : this.Parent.Root; } }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            _configuration = this.Configuration.Data as ShipPartConfiguration;

            // Call internal pre initialize functions
            this.ConnectionNode_PreInitialize();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();
        }

        public override void Dispose()
        {
            base.Dispose();

            // Call internal dispose functions
            this.ConnectionNode_Dispose();
            this.Farseer_Dispose();
        }
        #endregion
    }
}
