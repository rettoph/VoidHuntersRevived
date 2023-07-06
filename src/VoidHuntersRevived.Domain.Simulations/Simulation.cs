using Guppy.Common;
using Guppy.Common.Extensions;
using Guppy.Resources.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Physics.Factories;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations.Services;
using Guppy.Common.Providers;
using Svelto.ECS.Schedulers;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Simulations.EnginesGroups;
using VoidHuntersRevived.Common.Entities.Services;
using Serilog;
using VoidHuntersRevived.Common.Entities.Enums;
using Guppy.Network;

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract partial class Simulation : ISimulation, IDisposable
    {
        private readonly IServiceScope _scope;
        private readonly DrawEngineGroups _drawEnginesGroups;
        private Queue<EventDto> _events;

        public readonly SimulationType Type;
        public readonly IEngineService Engines;
        public readonly IEventPublishingService Events;

        SimulationType ISimulation.Type => this.Type;
        IServiceScope ISimulation.Scope => _scope;

        protected Simulation(SimulationType type, IServiceProvider provider)
        {
            this.Type = type;

            _scope = provider.CreateScope();

            // Pass the current scoped netscope to the new child scope
            _scope.ServiceProvider.GetRequiredService<ScopedProvider<NetScope>>().SetInstance(provider.GetRequiredService<NetScope>());
            this.Engines = _scope.ServiceProvider.GetRequiredService<IEngineService>().Load(new SimulationState(this));
            this.Events = _scope.ServiceProvider.GetRequiredService<IEventPublishingService>();

            _drawEnginesGroups = new DrawEngineGroups(this.Engines.OfType<IStepEngine<GameTime>>());
            _events = new Queue<EventDto>();
        }

        public virtual void Initialize(ISimulationService simulations)
        {
            this.Engines.Initialize();

            foreach(ISimulationEngine<ISimulation> engine in this.Engines.OfType<ISimulationEngine<ISimulation>>())
            {
                engine.Initialize(this);
            }
        }

        public virtual void Dispose()
        {
            this.Engines.Dispose();
        }

        public virtual void Draw(GameTime realTime)
        {
            _drawEnginesGroups.Step(realTime);
        }

        public virtual void Update(GameTime realTime)
        {
            while (this.TryGetNextStep(realTime, out Step? step))
            {
                this.DoStep(step);
            }
        }

        protected abstract bool TryGetNextStep(GameTime realTime, [MaybeNullWhen(false)] out Step step);
        protected virtual void DoStep(Step step)
        {
            this.Engines.Step(step);
        }

        public virtual void Publish(EventDto data)
        {
            this.Events.Publish(data);
        }

        public void Publish(VhId eventId, IEventData data)
        {
            this.Publish(new EventDto()
            {
                Id = eventId,
                Data = data
            });
        }

        protected virtual void Enqueue(EventDto @event)
        {
            _events.Enqueue(@event);
        }

        public virtual void Input(VhId eventId, IInputData data)
        {
            this.Publish(eventId, data);
        }
    }
}
