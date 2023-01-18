using Guppy.Network.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Helpers;

namespace VoidHuntersRevived.Common.Simulations.Extensions
{
    public static class UserExtensions
    {
        public static ParallelKey GetPilotKey(this User user)
        {
            return ParallelKeyHelper.GetPilotKey(user.Id);
        }
    }
}
