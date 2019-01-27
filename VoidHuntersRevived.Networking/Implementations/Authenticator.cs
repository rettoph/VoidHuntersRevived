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
        public AuthenticationResponse Authenticate(ServerPeer server, NetIncomingMessage im)
        {
            var response = new AuthenticationResponse(
                true,
                new User(
                    im.SenderConnection.RemoteUniqueIdentifier,
                    im.ReadString()));

            return response;
        }
    }
}
