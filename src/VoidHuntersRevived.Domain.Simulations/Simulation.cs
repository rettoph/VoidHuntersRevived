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
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;

using CreateEntityEvent = VoidHuntersRevived.Common.Entities.Events.CreateEntity;
using DestroyEntityEvent = VoidHuntersRevived.Common.Entities.Events.DestroyEntity;

namespace VoidHuntersRevived.Domain.Simulations
{
    public abstract partial class Simulation<TEntityComponent> : ISimulation, IDisposable
        where TEntityComponent : class, new()
    {
        private ISimulationUpdateSystem[] _updateSystems;
        private readonly IParallelEntityService _parallelables;
        private readonly TEntityComponent _entityComponent;
        private readonly IGlobalSimulationService _globalsSmulationService;
        private readonly Dictionary<ParallelKey, int> _keysToIds;

        public readonly SimulationType Type;
        public readonly ISpace Space;
        public readonly Type EntityComponentType;
        public IServiceProvider Provider;

        SimulationType ISimulation.Type => this.Type;
        ISpace ISimulation.Space => this.Space;
        Type ISimulation.EntityComponentType => this.EntityComponentType;
        IServiceProvider ISimulation.Provider => this.Provider;

        protected Simulation(
            SimulationType type,
            IParallelEntityService parallelables,
            ISpaceFactory spaceFactory,
            IGlobalSimulationService globalSimulationService)
        {
            _parallelables = parallelables;
            _updateSystems = Array.Empty<ISimulationUpdateSystem>();
            _entityComponent = new TEntityComponent();
            _globalsSmulationService = globalSimulationService;
            _keysToIds = new Dictionary<ParallelKey, int>();

            this.Type = type;
            this.Space = spaceFactory.Create();
            this.EntityComponentType = typeof(TEntityComponent);
            this.Provider = default!;
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

        public abstract ISimulationEvent Publish(SimulationEventData data);

        public abstract void Enqueue(SimulationEventData data);

        public bool TryGetEntityId(ParallelKey key, [MaybeNullWhen(false)] out int entityId)
        {
            return _keysToIds.TryGetValue(key, out entityId);
        }

        public int GetEntityId(ParallelKey key)
        {
            return _keysToIds[key];
        }

        public bool HasEntity(ParallelKey key)
        {
            return _keysToIds.ContainsKey(key);
        }

        public void Configure(Entity entity)
        {
            entity.Attach(_entityComponent);
            entity.Attach<ISimulation>(this);
        }

        public void Map(ParallelKey entityKey, int entityId)
        {
            _keysToIds.Add(entityKey, entityId);
        }

        public void Unmap(ParallelKey entityKey)
        {
            _keysToIds.Remove(entityKey);
        }
    }
}
