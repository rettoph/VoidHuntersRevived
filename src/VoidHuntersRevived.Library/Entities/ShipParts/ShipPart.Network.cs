using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Utilities;
using Guppy.Network.Extensions.Lidgren;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public partial class ShipPart
    {
        #region Read/Write All Methods
        public virtual void WriteCreate(NetOutgoingMessage om)
        {
            //
        }

        public virtual void ReadCreate(NetIncomingMessage im)
        {
            //
        }
        #endregion
    }
}
