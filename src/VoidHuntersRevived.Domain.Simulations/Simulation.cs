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
using VoidHuntersRevived.Domain.Simulations.Utilities;
using Guppy.Common.Services;
using Guppy;
using Guppy.Extensions.Autofac;
using Guppy.Game.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Extensions;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract partial class Simulation : ISimulation, IDisposable
    {
        private readonly Dictionary<Type, EventPublisher> _publishers;
        private IStepGroupEngine<GameTime> _drawEnginesGroup;

        protected readonly ILogger logger;

        public readonly SimulationType Type;
        public readonly IEngineService Engines;
        public readonly ITeamService Teams;
        public readonly ILifetimeScope Scope;

        public VhId Id { get; }
        public Step CurrentStep { get; private set; }

        SimulationType ISimulation.Type => this.Type;
        ILifetimeScope ISimulation.Scope => this.Scope;

        protected Simulation(SimulationType type, ILifetimeScope scope)
        {
            this.Id = HashBuilder<Simulation, ulong, SimulationType>.Instance.Calculate(scope.Resolve<IGuppy>().Id, type);
            this.Type = type;

            this.Scope = scope.BeginGuppyScope(nameof(Simulation), builder =>
            {
                builder.RegisterInstance<ISimulation>(this);

                builder.Configure<LoggerConfiguration>((scope, configuration) =>
                {
                    configuration.Enrich.WithProperty("PeerType", scope.Resolve<NetScope>().Peer?.Type ?? PeerType.None);
                    configuration.Enrich.WithProperty("SimulationType", this.Type);
                });
            });

            // Pass the current scoped netscope to the new child scope
            this.Engines = this.Scope.Resolve<IEngineService>();
            this.Teams = this.Scope.Resolve<ITeamService>();

            // Build an event publisher dictionary
            this.logger = this.Scope.Resolve<ILogger>();
            this._publishers = EventPublisher.BuildPublishers(this.Engines, this.logger);

            _drawEnginesGroup = this.Engines.All().CreateSequencedStepEnginesGroup<GameTime, DrawSequence>(DrawSequence.Draw);

            this.CurrentStep = new Step();
        }

        public virtual void Initialize(ISimulationService simulations)
        {
            this.Engines.Initialize();

            this.Engines.InitializeSimulationEngines(this);
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
            this.CurrentStep = step;
        }

        protected virtual void Revert(EventDto @event)
        {
            _publishers[@event.Data.GetType()].Revert(@event);
        }
        protected virtual void Publish(EventDto @event)
        {
            _publishers[@event.Data.GetType()].Publish(@event);
        }

        public void Publish(VhId sender, IEventData data)
        {
            this.Publish(new EventDto()
            {
                Sender = sender,
                Data = data
            });
        }

        public abstract void Input(VhId sender, IInputData data);
    }
}
