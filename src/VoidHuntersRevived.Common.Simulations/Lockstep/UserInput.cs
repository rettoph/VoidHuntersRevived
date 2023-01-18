using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Lockstep
{
    public sealed class UserInput
    {
        public readonly ParallelKey User;
        public readonly IData Data;

        public UserInput(ParallelKey user, IData data)
        {
            this.User = user;
            this.Data = data;
        }
    }
}
