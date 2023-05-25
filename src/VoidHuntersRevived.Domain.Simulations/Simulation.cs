using Guppy.Common;
using Guppy.Common.Collections;
using Guppy.ECS;
using Guppy.ECS.Services;
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
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Physics.Factories;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Providers;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Domain.Simulations.Providers;

using CreateEntityEvent = VoidHuntersRevived.Common.Entities.Events.CreateEntity;
using DestroyEntityEvent = VoidHuntersRevived.Common.Entities.Events.DestroyEntity;

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract partial class Simulation<TEntityComponent> : ISimulation, IDisposable
        where TEntityComponent : class, new()
    {
        private ISimulationUpdateSystem[] _updateSystems;
        private readonly IParallelableService _parallelables;
        private readonly TEntityComponent _entityComponent;
        private readonly IGlobalSimulationService _globalsSmulationService;

        public readonly SimulationType Type;
        public readonly ISpace Space;
        public readonly Type EntityComponentType;
        public IServiceProvider Provider;

        SimulationType ISimulation.Type => this.Type;
        ISpace ISimulation.Space => this.Space;
        Type ISimulation.EntityComponentType => this.EntityComponentType;
        IServiceProvider ISimulation.Provider => this.Provider;

        public IParallelKeyProvider Keys { get; }

        protected Simulation(
            SimulationType type,
            IParallelableService parallelables,
            ISpaceFactory spaceFactory,
            IGlobalSimulationService globalSimulationService)
        {
            _parallelables = parallelables;
            _updateSystems = Array.Empty<ISimulationUpdateSystem>();
            _entityComponent = new TEntityComponent();
            _globalsSmulationService = globalSimulationService;

            this.Type = type;
            this.Space = spaceFactory.Create();
            this.EntityComponentType = typeof(TEntityComponent);
            this.Provider = default!;
            this.Keys = new ParallelKeyProvider();
        }

        public virtual void Initialize(IServiceProvider provider)
        {
            this.Provider = provider;

            _updateSystems = this.Provider.GetRequiredService<ISorted<ISimulationUpdateSystem>>().ToArray();

            foreach(ISimulationSystem system in this.Provider.GetRequiredService<ISorted<ISimulationSystem>>())
            {
                system.Initialize(this);
            }

            _globalsSmulationService.Add(this);
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

        protected abstract void Update(GameTime gameTime);

        void ISimulation.Update(GameTime gameTime)
        {
            this.Update(gameTime);
        }

        public virtual int CreateEntity(ParallelKey key, EntityType type)
        {
            ISimulationEvent @event = this.Publish(new SimulationEventData()
            {
                Key = CreateEntityEvent.Noise.Merge(key),
                SenderId = default!,
                Body = new CreateEntityEvent(type, key, null)
            });

            ParallelKey entityKey = (ParallelKey)@event.Response!;
            return this.GetEntityId(entityKey);
        }

        public virtual int CreateEntity(ParallelKey key, EntityType type, Action<Entity> factory)
        {
            ISimulationEvent @event = this.Publish(new SimulationEventData()
            {
                Key = CreateEntityEvent.Noise.Merge(key),
                SenderId = default!,
                Body = new CreateEntityEvent(type, key, factory)
            });

            ParallelKey entityKey = (ParallelKey)@event.Response!;
            return this.GetEntityId(entityKey);
        }

        public void DestroyEntity(ParallelKey key)
        {
            ISimulationEvent @event = this.Publish(new SimulationEventData()
            {
                Key = DestroyEntityEvent.Noise.Merge(key),
                SenderId = default!,
                Body = new DestroyEntityEvent(key, true)
            });
        }

        public void ConfigureEntity(Entity entity)
        {
            entity.Attach(_entityComponent);
            entity.Attach<ISimulation>(this);
        }

        public abstract ISimulationEvent Publish(SimulationEventData data);

        public abstract void Enqueue(SimulationEventData data);
    }
}
