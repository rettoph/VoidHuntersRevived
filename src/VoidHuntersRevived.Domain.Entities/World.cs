using Svelto.ECS.Schedulers;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities.Systems;
using VoidHuntersRevived.Domain.Entities.Services;
using Guppy.Common;
using Guppy.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Abstractions;
using Guppy.Common.Extensions;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Entities
{
    public sealed class World : IWorld, IDisposable
    {
        private Action<Step> _stepEngines;
        private readonly IBus _bus;
        private readonly EnginesRoot _enginesRoot;
        private readonly SimpleEntitiesSubmissionScheduler _simpleEntitiesSubmissionScheduler;
        private readonly EntityTypeService _types;
        private readonly EntityService _entities;
        private readonly ComponentService _components;

        public IEntityService Entities => _entities;

        public IComponentService Components => _components;

        public ISystem[] Systems { get; private set; }

        public World(IBus bus, IFilteredProvider filtered, params IState[] states)
        {
            _bus = bus;
            _stepEngines = null!;
            _simpleEntitiesSubmissionScheduler = new SimpleEntitiesSubmissionScheduler();
            _enginesRoot = new EnginesRoot(_simpleEntitiesSubmissionScheduler);

            _types = filtered.Get<EntityTypeService>().Instance;
            _entities = new EntityService(_types, _enginesRoot.GenerateEntityFactory(), _enginesRoot.GenerateEntityFunctions());
            _components = new ComponentService(_entities);

            this.Systems = filtered.Instances<ISystem>(states).Sort().ToArray();

            this.InitializeEngines();
        }

        public void Initialize()
        {
            _bus.SubscribeMany(this.Systems.OfType<ISubscriber>());
        }

        public void Dispose()
        {
            _bus.UnsubscribeMany(this.Systems.OfType<ISubscriber>());
        }

        public void Step(Step step)
        {
            _simpleEntitiesSubmissionScheduler.SubmitEntities();

            _stepEngines(step);
        }

        private void InitializeEngines()
        {
            _enginesRoot.AddEngine(_entities);
            _enginesRoot.AddEngine(_components);

            foreach(IEngine engine in BuildReactiveEngines(_entities, this.Systems))
            {
                _enginesRoot.AddEngine(engine);
            }

            foreach (Abstractions.IStepEngine stepEngine in BuildStepEngines(_components, this.Systems))
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
                if (system is IStepSystem stepSystem)
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
