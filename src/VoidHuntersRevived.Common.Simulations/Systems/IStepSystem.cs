using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.ECS;

namespace VoidHuntersRevived.Common.Simulations.Systems
{
    public interface IStepSystem
    {
        void Step(Step step);
    }

    public interface IStepSystem<T1>
        where T1 : unmanaged
    {
        void Step(in EntityId id, ref T1 component1, Step step);
    }
}
