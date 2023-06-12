using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Domain.Simulations.Abstractions;
using VoidHuntersRevived.Domain.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations
{
    public partial class Simulation
    {
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
    }
}
