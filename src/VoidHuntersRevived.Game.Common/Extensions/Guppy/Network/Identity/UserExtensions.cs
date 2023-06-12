using Guppy.Network.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Game.Common.Components;

namespace Guppy.Network.Identity
{
    public static class UserExtensions
    {
        private static Guid PilotNameSpace = new Guid("5E0EAB9C-18B0-4AC7-9A0A-0813B6B636B1");

        public static Guid GetPilotId(this User user)
        {
            return PilotNameSpace.Create(0);
        }
    }
}
