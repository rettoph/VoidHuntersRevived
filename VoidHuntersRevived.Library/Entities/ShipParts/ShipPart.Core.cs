using GalacticFighters.Library.Configurations;
using Lidgren.Network;
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
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            // Call internal create functions
            this.ConnectionNode_Create(provider);
        }

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
        }
        #endregion

        #region Network Methods
        protected override void Read(NetIncomingMessage im)
        {
            base.Read(im);

            this.ConnectionNode_Read(im);
        }

        protected override void Write(NetOutgoingMessage om)
        {
            base.Write(om);

            this.ConnectionNode_Write(om);
        }
        #endregion
    }
}
