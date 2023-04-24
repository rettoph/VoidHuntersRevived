using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using Guppy.Network.Identity;
using LiteNetLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract partial class Simulation<TEntityComponent> : ISimulation, IDisposable,
        ISubscriber<Tick>
        where TEntityComponent : class, new()
    {
        private World _world;
        private ISimulationUpdateSystem[] _updateSystems;
        private readonly IBus _bus;
        private readonly IParallelableService _parallelables;
        private readonly TEntityComponent _entityComponent;
        private readonly IGlobalSimulationService _globalsSmulationService;

        public readonly SimulationType Type;
        public readonly Aether Aether;
        public readonly Type EntityComponentType;
        public IServiceProvider Provider;

        SimulationType ISimulation.Type => this.Type;
        Aether ISimulation.Aether => this.Aether;
        Type ISimulation.EntityComponentType => this.EntityComponentType;
        IServiceProvider ISimulation.Provider => this.Provider;

        protected Simulation(
            SimulationType type,
            IBus bus,
            IParallelableService parallelables,
            IGlobalSimulationService globalSimulationService)
        {
            _bus = bus;
            _parallelables = parallelables;
            _world = default!;
            _updateSystems = Array.Empty<ISimulationUpdateSystem>();
            _entityComponent = new TEntityComponent();
            _globalsSmulationService = globalSimulationService;

            this.Type = type;
            this.Aether = new Aether(Vector2.Zero);
            this.EntityComponentType = typeof(TEntityComponent);
            this.Provider = default!;
        }

        public virtual void Initialize(IServiceProvider provider)
        {
            this.Provider = provider;

            _world = this.Provider.GetRequiredService<World>();
            _updateSystems = this.Provider.GetRequiredService<IFiltered<ISimulationUpdateSystem>>().Instances.ToArray();

            foreach(ISimulationSystem system in this.Provider.GetRequiredService<IFiltered<ISimulationSystem>>().Instances)
            {
                system.Initialize(this);
            }

            _globalsSmulationService.Add(this);

            this.CreateEntity(ParallelKeys.System);
        }

        public void Dispose()
        {
            _globalsSmulationService.Remove(this);
        }

        protected virtual void UpdateSystems(GameTime gameTime)
        {
            foreach (ISimulationUpdateSystem updateSystem in _updateSystems)
            {
                updateSystem.Update(this, gameTime);
            }
        }

        public bool TryGetEntityId(ParallelKey key, [MaybeNullWhen(false)] out int id)
        {
            return _parallelables.Get(key).TryGetId(this.Type, out id);
        }

        public int GetEntityId(ParallelKey key)
        {
            if (_parallelables.Get(key).TryGetId(this.Type, out var id))
            {
                return id;
            }

            throw new InvalidOperationException();
        }

        public bool HasEntity(ParallelKey key)
        {
            return _parallelables.Get(key).TryGetId(this.Type, out _);
        }

        public void DestroyEntity(ParallelKey key)
        {
            Parallelable parallelable = _parallelables.Get(key);

            if(parallelable.TryGetId(this.Type, out int id))
            {
                parallelable.RemoveId(this);
                _world.DestroyEntity(id);
            }
        }

        protected abstract void Update(GameTime gameTime);

        void ISimulation.Update(GameTime gameTime)
        {
            this.Update(gameTime);
        }

        public virtual Entity CreateEntity(ParallelKey key)
        {
            var parallelable = _parallelables.Get(key);
            var entity = _world.CreateEntity();
            parallelable.AddId(this, entity.Id);

            entity.Attach(parallelable);
            entity.Attach(_entityComponent);
            entity.Attach<ISimulation>(this);

            return entity;
        }

        public abstract void Input(ParallelKey sender, IData data);

        public void Publish(IInput @event)
        {
            _bus.Publish(@event);
        }


        public void Process(in Tick message)
        {
            foreach (EventDto @eventDto in message.Events)
            {
                IInput @event = Simulation.Event.Factory.Create(SimulationType.Lockstep, @eventDto.Sender, @eventDto.Data, this);
                this.Publish(@event);
            }
        }
    }
}
