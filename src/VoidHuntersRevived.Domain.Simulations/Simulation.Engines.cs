using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Domain.Simulations.Abstractions;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations
{
    public partial class Simulation
    {
        private Action<Step> _stepEngines = null!;
        private Action<Tick> _tickSystems = null!;

        private void InitializeEngines()
        {
            IEnumerable<IEngine> engines = Enumerable.Empty<IEngine>()
            .Concat(new IEngine[]
            {
                _entities,
                _components
            })
            .Concat(this.Systems.Select(x => new SystemEngine(this, x)))
            .Concat(BuildReactiveEngines(_entities, this.Systems));

            foreach (IEngine engine in engines)
            {
                _enginesRoot.AddEngine(engine);
            }

            foreach(Abstractions.IStepEngine stepEngine in BuildStepEngines(_components, this.Systems))
            {
                _stepEngines += stepEngine.Step;
                _enginesRoot.AddEngine(stepEngine);
            }
        }

        private static IEnumerable<IEngine> BuildReactiveEngines(EntityService entities, ISystem[] systems)
        {
            foreach (ISystem system in systems)
            {
                foreach (Type interfaceType in system.GetType().GetInterfaces())
                {
                    if (interfaceType.IsConstructedGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IReactiveSystem<>))
                    {
                        Type engineType = typeof(ReactiveEngine<>).MakeGenericType(interfaceType.GenericTypeArguments[0]);
                        object? engine = Activator.CreateInstance(engineType, entities, system);

                        yield return engine as IEngine ?? throw new Exception();
                    }
                }
            }
        }

        private static IEnumerable<Abstractions.IStepEngine> BuildStepEngines(ComponentService components, ISystem[] systems)
        {
            foreach (ISystem system in systems)
            {
                if(system is IStepSystem stepSystem)
                {
                    yield return new StepEngine(stepSystem);
                }

                foreach (Type interfaceType in system.GetType().GetInterfaces())
                {
                    if (interfaceType.IsConstructedGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IStepSystem<>))
                    {
                        Type engineType = typeof(StepEngine<>).MakeGenericType(interfaceType.GenericTypeArguments);
                        object? engine = Activator.CreateInstance(engineType, components, system);

                        yield return engine as Abstractions.IStepEngine ?? throw new Exception();
                    }
                }

                foreach (Type interfaceType in system.GetType().GetInterfaces())
                {
                    if (interfaceType.IsConstructedGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IStepSystem<,>))
                    {
                        Type engineType = typeof(StepEngine<,>).MakeGenericType(interfaceType.GenericTypeArguments);
                        object? engine = Activator.CreateInstance(engineType, components, system);

                        yield return engine as Abstractions.IStepEngine ?? throw new Exception();
                    }
                }

                foreach (Type interfaceType in system.GetType().GetInterfaces())
                {
                    if (interfaceType.IsConstructedGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IStepSystem<,,>))
                    {
                        Type engineType = typeof(StepEngine<,,>).MakeGenericType(interfaceType.GenericTypeArguments);
                        object? engine = Activator.CreateInstance(engineType, components, system);

                        yield return engine as Abstractions.IStepEngine ?? throw new Exception();
                    }
                }

                foreach (Type interfaceType in system.GetType().GetInterfaces())
                {
                    if (interfaceType.IsConstructedGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IStepSystem<,,,>))
                    {
                        Type engineType = typeof(StepEngine<,,,>).MakeGenericType(interfaceType.GenericTypeArguments);
                        object? engine = Activator.CreateInstance(engineType, components, system);

                        yield return engine as Abstractions.IStepEngine ?? throw new Exception();
                    }
                }
            }
        }
    }
}
