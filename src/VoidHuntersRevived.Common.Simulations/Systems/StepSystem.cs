using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.ECS;

namespace VoidHuntersRevived.Common.Simulations.Systems
{
    public class StepSystem<T1> : BasicSystem, IStepSystem
        where T1 : unmanaged
    {
        public void Step(Step step)
        {
        }
    }
}
