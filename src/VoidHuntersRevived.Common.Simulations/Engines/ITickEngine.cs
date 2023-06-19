using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Common.Simulations.Engines
{
    public interface ITickEngine : IStepEngine<Tick>
    {
    }
}
