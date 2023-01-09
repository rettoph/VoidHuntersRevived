using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Services
{
    public interface ITickService
    {
        bool TryTick();
    }
}
