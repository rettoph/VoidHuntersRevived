using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Networking.Interfaces;

namespace VoidHuntersRevived.Networking.Responses
{
    public class AuthenticationResponse
    {
        public Boolean Authenticated { get; private set; }
        public IUser User { get; private set; }

        public AuthenticationResponse(Boolean authenticated = false, IUser user = null)
        {
            this.Authenticated = authenticated;
            this.User = user;
        }
    }
}
