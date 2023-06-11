using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Systems
{
    public interface IStepSystem
    {
        void Step(Step step);
    }
}
