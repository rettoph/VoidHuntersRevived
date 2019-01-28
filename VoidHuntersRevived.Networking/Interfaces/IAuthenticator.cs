using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Networking.Peers;
using VoidHuntersRevived.Networking.Responses;

namespace VoidHuntersRevived.Networking.Interfaces
{
    /// <summary>
    /// Authenticators can be used to validate incomming connections
    /// Simply put, they are given the clients remote hail, and return
    /// an Authentication response instance
    /// </summary>
    public interface IAuthenticator
    {
        AuthenticationResponse Authenticate(ServerPeer server, NetConnection connection);
    }
}
