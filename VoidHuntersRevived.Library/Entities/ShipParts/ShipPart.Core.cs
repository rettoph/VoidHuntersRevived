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

        #region Lifecycle Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            _configuration = this.Configuration.Data as ShipPartConfiguration;
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

            this.Farseer_Dispose();
        }
        #endregion
    }
}
