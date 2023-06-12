using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Abstractions
{
    [AllowMultiple]
    public interface IStepEngine : IEngine
    {
        void Step(Step step);
    }

    internal class StepEngine : IStepEngine
    {
        private readonly IStepSystem _system;

        public StepEngine(IStepSystem system)
        {
            _system = system;
        }

        public void Step(Step step)
        {
            _system.Step(step);
        }
    }

    internal class StepEngine<T1> : IStepEngine
        where T1 : unmanaged
    {
        private readonly IStepSystem<T1> _system;
        private readonly IComponentService _components;

        public StepEngine(IComponentService components, IStepSystem<T1> system)
        {
            _system = system;
            _components = components;
        }

        public void Step(Step step)
        {
            _components.Iterate<T1>(_system.Step, step);
        }
    }

    internal class StepEngine<T1, T2> : IStepEngine
        where T1 : unmanaged
        where T2 : unmanaged
    {
        private readonly IStepSystem<T1, T2> _system;
        private readonly IComponentService _components;

        public StepEngine(IComponentService components, IStepSystem<T1, T2> system)
        {
            _system = system;
            _components = components;
        }

        public void Step(Step step)
        {
            _components.Iterate<T1, T2>(_system.Step, step);
        }
    }

    internal class StepEngine<T1, T2, T3> : IStepEngine
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
    {
        private readonly IStepSystem<T1, T2, T3> _system;
        private readonly IComponentService _components;

        public StepEngine(IComponentService components, IStepSystem<T1, T2, T3> system)
        {
            _system = system;
            _components = components;
        }

        public void Step(Step step)
        {
            _components.Iterate<T1, T2, T3>(_system.Step, step);
        }
    }

    internal class StepEngine<T1, T2, T3, T4> : IStepEngine
        where T1 : unmanaged
        where T2 : unmanaged
        where T3 : unmanaged
        where T4 : unmanaged
    {
        private readonly IStepSystem<T1, T2, T3, T4> _system;
        private readonly IComponentService _components;

        public StepEngine(IComponentService components, IStepSystem<T1, T2, T3, T4> system)
        {
            _system = system;
            _components = components;
        }

        public void Step(Step step)
        {
            _components.Iterate<T1, T2, T3, T4>(_system.Step, step);
        }
    }
}
