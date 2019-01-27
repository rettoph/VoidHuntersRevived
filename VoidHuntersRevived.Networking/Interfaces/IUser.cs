using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Networking.Interfaces
{
    public interface IUser : INetworkObject, IDisposable
    {
        String Name { get; }

        event EventHandler<IUser> OnDisconnect;

        /// <summary>
        /// Mark the current user as disconnected.
        /// This is a one time action, and will trigger
        /// the OnDisconnect event.
        /// </summary>
        void Disconnect();
    }
}
