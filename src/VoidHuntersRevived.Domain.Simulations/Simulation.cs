using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using Autofac;
using Guppy.Common.Extensions.Autofac;
using Serilog;
using Guppy.Network;
using Guppy.Common;
using Guppy.Network.Enums;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Simulations.Enums;

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract partial class Simulation : ISimulation, IDisposable
    {
        private Queue<EventDto> _events;
        private IStepGroupEngine<GameTime> _drawEnginesGroup;

        public readonly SimulationType Type;
        public readonly IEngineService Engines;
        public readonly IEventPublishingService Events;
        public readonly ILifetimeScope Scope;

        SimulationType ISimulation.Type => this.Type;
        ILifetimeScope ISimulation.Scope => this.Scope;

        protected Simulation(SimulationType type, ILifetimeScope scope)
        {
            this.Type = type;

            this.Scope = scope.BeginLifetimeScope(builder =>
            {
                builder.Configure<LoggerConfiguration>(configuration =>
                {
                    configuration.Enrich.WithProperty("PeerType", scope.Resolve<NetScope>().Peer?.Type ?? PeerType.None);
                    configuration.Enrich.WithProperty("SimulationType", this.Type);
                });
            });

            // Pass the current scoped netscope to the new child scope
            this.Engines = this.Scope.Resolve<IEngineService>().Load(new SimulationState(this));
            this.Events = this.Scope.Resolve<IEventPublishingService>();

            _drawEnginesGroup = this.Engines.All().CreateSequencedStepEnginesGroup<GameTime, DrawEngineSequence>(DrawEngineSequence.Draw);
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
            _drawEnginesGroup.Step(realTime);
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

        public void Publish(VhId sender, IEventData data)
        {
            this.Events.Publish(sender, data);
        }

        protected virtual void Enqueue(EventDto @event)
        {
            _events.Enqueue(@event);
        }

        public abstract void Input(VhId sender, IInputData data);
    }
}
