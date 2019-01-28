using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using VoidHuntersRevived.Networking.Interfaces;
using VoidHuntersRevived.Networking.Peers;
using VoidHuntersRevived.Networking.Responses;

namespace VoidHuntersRevived.Networking.Implementations
{
    /// <summary>
    /// Simple instance of an authenticator that will simpl accept any
    /// uncoming connection
    /// </summary>
    public class Authenticator : IAuthenticator
    {
        public AuthenticationResponse Authenticate(ServerPeer server, NetConnection connection)
        {
            var name = connection.RemoteHailMessage.ReadString();
            var response = new AuthenticationResponse(
                true,
                new User(
                    connection.RemoteUniqueIdentifier,
                    name));

            return response;
        }
    }
}
