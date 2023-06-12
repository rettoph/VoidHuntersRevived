using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Simulations.Systems
{
    public interface IStepSystem
    {
        void Step(Step step);
    }

    public interface IStepSystem<T1>
        where T1 : unmanaged
    {
        void Step(Step step, in EntityId id, ref T1 component1);
    }

    public interface IStepSystem<T1, T2>
        where T1 : unmanaged
        where T2 : unmanaged
    {
        void Step(Step step, in EntityId id, ref T1 component1, ref T2 component2);
    }

    public interface IStepSystem<T1, T2, T3>
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        void Step(Step step, in EntityId id, ref T1 component1, ref T2 component2, ref T3 component3);
    }

    public interface IStepSystem<T1, T2, T3, T4>
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
    {
        void Step(Step step, in EntityId id, ref T1 component1, ref T2 component2, ref T3 component3, ref T4 component4);
    }
}
