using Guppy.DependencyInjection;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Entities.ShipParts
{
    public partial class ShipPart
    {
        #region Private Fields
        private ShipPartService _shipParts;
        #endregion

        #region Lifecycle Methods
        private void Network_Create(ServiceProvider provider)
        {
            provider.Service(out _shipParts);

            this.MessageHandlers[MessageType.Create].Add(this.ReadContext, this.WriteContext);
        }

        private void Network_Dispose()
        {
            _shipParts = null;

            this.MessageHandlers[MessageType.Create].Remove(this.ReadContext, this.WriteContext);
        }
        #endregion

        #region Network Methods
        private void ReadContext(NetIncomingMessage im)
            => this.SetContext(_shipParts[im.ReadUInt32()]);

        private void WriteContext(NetOutgoingMessage om)
            => om.Write(this.Context.Id);
        #endregion
    }
}
