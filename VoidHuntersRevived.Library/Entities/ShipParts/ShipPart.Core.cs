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
    public abstract partial class ShipPart
    {
        #region Protected Fields
        protected ShipPartConfiguration config;
        #endregion

        #region Public Attributes

        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

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
