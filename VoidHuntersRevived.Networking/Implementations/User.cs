using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Networking.Implementations
{
    public class User : IUser
    {
        public Int64 Id { get; private set; }
        public String Name { get; private set; }

        public event EventHandler<IUser> OnDisconnect;

        public User(Int64 id, String name = "")
        {
            this.Id = id;
            this.Name = name;
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            this.OnDisconnect?.Invoke(this, this);
            this.Dispose();
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        #region INetworkObject Implementation
        public void Read(NetIncomingMessage im)
        {
            this.Name = im.ReadString();
        }

        public void Write(NetOutgoingMessage om)
        {
            om.Write(this.Id);
            om.Write(this.Name);
        }
        #endregion
    }
}
