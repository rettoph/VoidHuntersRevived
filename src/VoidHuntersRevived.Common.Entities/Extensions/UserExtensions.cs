using Guppy.Network.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.Extensions
{
    public static class UserExtensions
    {
        public static ParallelKey GetKey(this User user)
        {
            return ParallelEntityTypes.Pilot.Create(user.Id);
        }
    }
}
