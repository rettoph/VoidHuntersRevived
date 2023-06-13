﻿using Guppy.Common;
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
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Domain.Simulations.Services;
using Guppy.Common.Providers;
using Svelto.ECS.Schedulers;
using Svelto.ECS;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Domain.Pieces.Services;

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract partial class Simulation : ISimulation, IDisposable
    {
        private readonly IBus _bus;
        private readonly EnginesRoot _enginesRoot;
        private readonly SimpleEntitiesSubmissionScheduler _simpleEntitiesSubmissionScheduler;
        private readonly EntityTypeService _types;
        private readonly EntityService _entities;
        private readonly ComponentService _components;

        protected readonly EventPublishingService publisher;

        public readonly SimulationType Type;
        public readonly ISpace Space;
        public readonly ISystem[] Systems;
        public readonly IPieceService Pieces;

        SimulationType ISimulation.Type => this.Type;
        ISpace ISimulation.Space => this.Space;
        IEntityService ISimulation.Entities => _entities;
        IComponentService ISimulation.Components => _components;
        ISystem[] ISimulation.Systems => this.Systems;
        IPieceService ISimulation.Pieces => this.Pieces;

        public Tick CurrentTick { get; private set; }

        protected Simulation(
            SimulationType type,
            ISpaceFactory spaceFactory,
            IFilteredProvider filtered,
            IBus bus)
        {
            _simpleEntitiesSubmissionScheduler = new SimpleEntitiesSubmissionScheduler();
            _enginesRoot = new EnginesRoot(_simpleEntitiesSubmissionScheduler);

            _types = filtered.Get< EntityTypeService>().Instance;
            _entities = new EntityService(_simpleEntitiesSubmissionScheduler, _types, _enginesRoot.GenerateEntityFactory(), _enginesRoot.GenerateEntityFunctions());
            _components = new ComponentService(_entities);
            _bus = bus;

            this.Type = type;
            this.Space = spaceFactory.Create();
            this.CurrentTick = Tick.First();
            this.Systems = filtered.Instances<ISystem>(new SimulationState(this)).Sort().ToArray();
            this.Pieces = new PieceService();

            this.publisher = new EventPublishingService(this.Systems);

            foreach(ITickSystem subscriber in this.Systems.OfType<ITickSystem>())
            {
                _tickSystems += subscriber.Tick;
            }
        }

        public virtual void Initialize(ISimulationService simulations)
        {
            this.InitializeEngines();

            _bus.SubscribeMany(this.Systems.OfType<ISubscriber>());
        }

        public virtual void Dispose()
        {
            _bus.UnsubscribeMany(this.Systems.OfType<ISubscriber>());
        }

        public virtual void Update(GameTime realTime)
        {
            while (this.TryGetNextStep(realTime, out Step? step))
            {
                this.DoStep(step);
            }

            if(this.TryGetNextTick(this.CurrentTick, out Tick? next))
            {
                this.DoTick(next);
            }
        }

        protected abstract bool TryGetNextStep(GameTime realTime, [MaybeNullWhen(false)] out Step step);
        protected virtual void DoStep(Step step)
        {
            _stepEngines(step);
        }

        protected abstract bool TryGetNextTick(Tick current, [MaybeNullWhen(false)] out Tick next);
        protected virtual void DoTick(Tick tick)
        {
            this.CurrentTick = tick;

            _tickSystems?.Invoke(tick);

            foreach(EventDto @event in tick.Events)
            {
                this.Publish(@event);
            }
        }

        public abstract void Publish(EventDto data);

        public abstract void Enqueue(EventDto data);
    }
}
