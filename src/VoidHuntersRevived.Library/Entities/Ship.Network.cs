using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Extensions.Collections;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy.Extensions.System;
using Guppy.Utilities;
using Guppy.DependencyInjection;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Entities
{
	public partial class Ship
    {
        #region Lifecycle Methods
        private void Network_Create(ServiceProvider provider)
        {
            this.MessageHandlers[MessageType.Setup].Add(this.ReadBridge, this.WriteBridge);
            this.MessageHandlers[MessageType.Setup].Add(this.ReadDirections, this.WriteDirections);
            this.MessageHandlers[MessageType.Setup].Add(this.ReadTarget, this.WriteTarget);
        }

        private void Network_Dispose()
        {
            this.MessageHandlers[MessageType.Setup].Remove(this.ReadBridge, this.WriteBridge);
            this.MessageHandlers[MessageType.Setup].Remove(this.ReadDirections, this.WriteDirections);
            this.MessageHandlers[MessageType.Setup].Remove(this.ReadTarget, this.WriteTarget);
        }
        #endregion

        internal void ReadBridge(NetIncomingMessage im)
            => this.SetBridge(im.ReadEntity<ShipPart>(_entities));

        internal void ReadDirections(NetIncomingMessage im)
        {
            for (Int32 i = 0; i < EnumHelper.Count<Ship.Direction>(); i++)
                this.ReadDirection(im);
        }

        internal void ReadDirection(NetIncomingMessage im)
            => this.TrySetDirection((Ship.Direction)im.ReadByte(), im.ReadBoolean());

        internal void ReadTarget(NetIncomingMessage im)
            => this.Target = im.ReadVector2();

        internal void WriteBridge(NetOutgoingMessage om)
            => om.Write(this.Bridge);

        internal void WriteDirections(NetOutgoingMessage om)
            => EnumHelper.GetValues<Ship.Direction>().ForEach(d =>
            {
                this.WriteDirection(om, d);
            });

        internal void WriteDirection(NetOutgoingMessage om, Ship.Direction direction)
        {
            om.Write((Byte)direction);
            om.Write((this.ActiveDirections & direction) != 0);
        }

        internal void WriteTarget(NetOutgoingMessage om)
            => om.Write(this.Target);
    }
}
