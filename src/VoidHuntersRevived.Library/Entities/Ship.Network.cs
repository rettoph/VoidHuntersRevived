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

namespace VoidHuntersRevived.Library.Entities
{
	public partial class Ship
    {
        #region Lifecycle Methods
        private void Network_Create(ServiceProvider provider)
        {
            this.OnWrite += this.WriteBridge;
            this.OnWrite += this.WriteDirections;
            this.OnWrite += this.WriteTarget;

            this.OnRead += this.ReadBridge;
            this.OnRead += this.ReadDirections;
            this.OnRead += this.ReadTarget;
        }

        private void Network_Dispose()
        {
            this.OnWrite -= this.WriteBridge;
            this.OnWrite -= this.WriteDirections;
            this.OnWrite -= this.WriteTarget;

            this.OnRead -= this.ReadBridge;
            this.OnRead -= this.ReadDirections;
            this.OnRead -= this.ReadTarget;
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
