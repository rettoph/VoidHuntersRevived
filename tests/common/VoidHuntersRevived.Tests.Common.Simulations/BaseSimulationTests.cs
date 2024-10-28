using Guppy.Core.Resources.Common;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Common.Constants;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Tests.Common.Extensions;
using VoidHuntersRevived.Tests.Common.Simulations.Extensions;

namespace VoidHuntersRevived.Tests.Common.Simulations
{
    public abstract class BaseSimulationTests<TSelf>
        where TSelf : BaseSimulationTests<TSelf>
    {
        public virtual SettingValue<int> StepsPerTick => new(Settings.StepsPerTick, 3);
        public virtual SettingValue<Fix64> StepInterval => new(Settings.StepInterval, (Fix64)20 / (Fix64)1000);

        private int _sourceIdGeneratorIndex;
        private readonly SimulationMocker _simulation;
        private readonly List<EventDto> _inputs;

        private readonly IStrategyMocker<ILockstepStrategy> _lockstep;
        private readonly IStrategyMocker<PredictiveStrategy>[] _predictives;
        protected SimulationMocker simulation => _simulation;

        public BaseSimulationTests()
        {
            _sourceIdGeneratorIndex = 0;

            _inputs = [];
            _simulation = new SimulationBuilder(
                id: VhId.Empty,
                stepInterval: StepInterval,
                stepsPerTick: StepsPerTick,
                entityTemplateFragments: this.GetEntityTemplateFragments(),
                engines: this.GetEngines
            ).AddPredictiveStrategy().AddLockstepClientStrategy().Build();

            _lockstep = _simulation.Strategies.OfType<IStrategyMocker<ILockstepStrategy>>().Single();
            _predictives = _simulation.Strategies.OfType<IStrategyMocker<PredictiveStrategy>>().ToArray();
        }

        public void Dispose()
        {
            _simulation.Dispose();
        }

        protected TSelf Input<T>(T input, bool verified)
            where T : IInputData
        {
            VhId sourceId = this.GenerateSourceId();

            foreach (IStrategyMocker<PredictiveStrategy> predictive in _predictives)
            {
                predictive.Instance.Input(sourceId, input);
            }

            if (verified == false)
            { // Simulate the "discarding" of a lockstep event - as if the server rejected the event.
                return (TSelf)this;
            }

            _inputs.Add(new EventDto()
            {
                SourceId = sourceId,
                Data = input
            });

            return (TSelf)this;
        }

        protected TSelf InputMany<T>(Func<int, T> inputGenerator, int count, int offset, bool verified)
            where T : IInputData
        {
            for (int i = 0; i < count; i++)
            {
                this.Input(inputGenerator(i + offset), verified);
            }

            return (TSelf)this;
        }

        protected TSelf Update(int interval, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (_lockstep.Instance.StepsSinceTick == _lockstep.Instance.StepsPerTick)
                {
                    _lockstep.Services.Get<TickBuffer>().TryEnqueue(Tick.Create(_lockstep.Instance.CurrentTick.Id + 1, _inputs.ToArray()));
                    _inputs.Clear();
                }

                this.simulation.Instance.Update(_simulation.GameTime.Step(interval));
            }

            return (TSelf)this;
        }

        private IReadOnlyDictionary<Key<IEntityTemplate>, EntityTemplateFragment[]> GetEntityTemplateFragmentsDictionary()
        {
            return this.GetEntityTemplateFragments()
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.ToArray());
        }

        private EntityTemplateFragment[] GetEntityTemplateFragmentsByKey(Key<IEntityTemplate> key)
        {
            return this.GetEntityTemplateFragments().Where(x => x.Key == key).ToArray();
        }

        protected abstract IEnumerable<EntityTemplateFragment> GetEntityTemplateFragments();

        protected abstract IEnumerable<IEngine> GetEngines();

        protected virtual VhId GenerateSourceId()
        {
            return HashBuilder<BaseSimulationTests<TSelf>, int>.Instance.Calculate(_sourceIdGeneratorIndex++);
        }

        protected Dictionary<IStrategy, int> CalculateTotalEntities<T>()
            where T : unmanaged, IEntityComponent
        {
            return _simulation.Strategies.ToDictionary(x => x.Instance, x => x.Services.Get<IEntityQueryService>().CalculateTotal<T>());
        }
    }
}
