using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts.Hulls;
using VoidHuntersRevived.Library.Entities.ShipParts.Thrusters;
using VoidHuntersRevived.Library.Entities.WorldObjects;

namespace VoidHuntersRevived.Library.Globals.Constants
{
    public static class ServiceConfigurationKeys
    {
        public static readonly ServiceConfigurationKey Chain = ServiceConfigurationKey.From<Chain>();

        public static class ShipParts
        {
            public static readonly ServiceConfigurationKey Hull = ServiceConfigurationKey.From<Hull>();
            public static readonly ServiceConfigurationKey Thruster = ServiceConfigurationKey.From<Thruster>();
        }

        public static class Players
        {
            public static readonly ServiceConfigurationKey UserPlayer = ServiceConfigurationKey.From<UserPlayer>();
        }
    }
}
