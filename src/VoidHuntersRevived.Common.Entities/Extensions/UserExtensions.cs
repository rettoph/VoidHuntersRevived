using Guppy.Network.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Extensions
{
    public static class UserExtensions
    {
        public static ParallelKey GetPilotKey(this User user)
        {
            return ParallelTypes.Pilot.Create(user.Id);
        }
    }
}
