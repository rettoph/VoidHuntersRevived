using Guppy.Network;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Groups;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Utilities
{
    /// <summary>
    /// Simple helper class used to store network read/write messages.
    /// </summary>
    public class NetworkEntityMessageTypeHandler
    {
        private NetworkEntity _parent;
        private MessageType _type;

        public NetIncomingMessageDelegate OnRead;
        public NetOutgoingMessageDelegate OnWrite;

        internal NetworkEntityMessageTypeHandler(MessageType type, NetworkEntity parent)
        {
            _type = type;
            _parent = parent;
        }

        public void TryRead(NetIncomingMessage im)
            => this.OnRead?.Invoke(im);

        public void TryWrite(NetOutgoingMessage om)
        {
            om.Write(VHR.Network.Pings.Scene.Entity, m =>
            {
                // The next two lines are manually read within the slave scene driver...
                m.Write((Byte)_type);
                m.Write(_parent.Id);

                // Write needed data...
                this.OnWrite?.Invoke(m);
            });
        }

        public void Add(NetIncomingMessageDelegate reader, NetOutgoingMessageDelegate writer)
        {
            this.OnRead += reader;
            this.OnWrite += writer;
        }

        public void Remove(NetIncomingMessageDelegate reader, NetOutgoingMessageDelegate writer)
        {
            this.OnRead -= reader;
            this.OnWrite -= writer;
        }
    }
}
