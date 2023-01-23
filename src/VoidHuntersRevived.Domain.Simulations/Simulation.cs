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
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract partial class Simulation<TEntityComponent> : ISimulation, IDisposable
        where TEntityComponent : class, new()
    {
        private IBus _bus;
        private World _world;
        private ISimulationUpdateSystem[] _updateSystems;
        private readonly IParallelService _parallel;
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
            IParallelService simulatedEntities,
            IGlobalSimulationService globalSimulationService)
        {
            _parallel = simulatedEntities;
            _world = default!;
            _bus = default!;
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
            _bus = this.Provider.GetRequiredService<IBus>();
            _updateSystems = this.Provider.GetRequiredService<IFiltered<ISimulationUpdateSystem>>().Instances.ToArray();

            _globalsSmulationService.Add(this);
        }

        public virtual void PostInitialize()
        {
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
            return _parallel.TryGetEntityIdFromKey(key, this.Type, out id);
        }

        public int GetEntityId(ParallelKey key)
        {
            if (_parallel.TryGetEntityIdFromKey(key, this.Type, out var id))
            {
                return id;
            }

            var entity = this.CreateEntity(key);

            return entity.Id;
        }

        public bool TryGetEntityId(int id, SimulationType toType, [MaybeNullWhen(false)] out int toId)
        {
            return _parallel.TryGetEntityId(id, toType, out toId);
        }

        public int GetEntityId(int id, SimulationType to)
        {
            return _parallel.GetId(id, to);
        }

        public bool TryGetEntity(ParallelKey key, [MaybeNullWhen(false)] out Entity entity)
        {
            if (_parallel.TryGetEntityIdFromKey(key, this.Type, out var id))
            {
                entity = _world.GetEntity(id);
                return true;
            }

            entity = null;
            return false;
        }

        public Entity GetEntity(ParallelKey key)
        {
            if (_parallel.TryGetEntityIdFromKey(key, this.Type, out var entityId))
            {
                return _world.GetEntity(entityId);
            }

            var entity = this.CreateEntity(key);

            return entity;
        }

        public bool HasEntity(ParallelKey key)
        {
            return _parallel.TryGetEntityIdFromKey(key, this.Type, out _);
        }

        public void AddEntity(ParallelKey key, Entity entity)
        {
            entity.Attach(_entityComponent);
            entity.Attach(new Parallelable(key));
            entity.Attach<ISimulation>(this);

            _parallel.Set(key, this.Type, entity.Id);
        }

        public void RemoveEntity(ParallelKey key, out Entity entity)
        {
            entity = this.GetEntity(key);
            _parallel.Remove(key, this.Type);
        }

        protected abstract void Update(GameTime gameTime);

        void ISimulation.Update(GameTime gameTime)
        {
            this.Update(gameTime);
        }

        protected virtual void PublishEvent(IEvent @event)
        {
            _bus.Publish(@event);
        }

        public virtual void PublishEvent(IData data)
        {
            this.PublishEvent(Simulation.Event.Factory.Create(this.Type, data, this));
        }

        public abstract void Input(ParallelKey user, IData data);
    }
}
