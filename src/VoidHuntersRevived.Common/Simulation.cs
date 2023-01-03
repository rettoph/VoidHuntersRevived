using Guppy.Common;
using Guppy.Network.Enums;
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
using VoidHuntersRevived.Common.Services;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Common
{
    public abstract class Simulation : ISimulation
    {
        private IBus _bus;
        private World _world;
        private IUpdateSimulationSystem[] _updateSystems;
        private readonly ISimulatedService _simulatedEntities;

        public readonly SimulationType Type;
        public readonly Aether Aether;

        SimulationType ISimulation.Type => this.Type;
        Aether ISimulation.Aether => this.Aether;

        protected Simulation(SimulationType type, ISimulatedService simulatedEntities)
        {
            _simulatedEntities = simulatedEntities;
            _world = default!;
            _bus = default!;
            _updateSystems = Array.Empty<IUpdateSimulationSystem>();

            this.Type = type;
            this.Aether = new Aether(Vector2.Zero);
        }

        public virtual void Initialize(IServiceProvider provider)
        {
            _world = provider.GetRequiredService<World>();
            _bus = provider.GetRequiredService<IBus>();
            _updateSystems = provider.GetRequiredService<IFiltered<IUpdateSimulationSystem>>().Instances.ToArray();
        }

        protected virtual void UpdateSystems(GameTime gameTime)
        {
            foreach (IUpdateSimulationSystem updateSystem in _updateSystems)
            {
                updateSystem.Update(this, gameTime);
            }
        }

        public bool TryGetEntityId(SimulatedId id, [MaybeNullWhen(false)] out int entityId)
        {
            return _simulatedEntities.TryGetEntityId(id, this.Type, out entityId);
        }

        public int GetEntityId(SimulatedId id)
        {
            if (_simulatedEntities.TryGetEntityId(id, this.Type, out var entityId))
            {
                return entityId;
            }

            var entity = this.CreateEntity(id);

            return entity.Id;
        }

        public bool TryGetEntityId(int entityId, SimulationType to, [MaybeNullWhen(false)] out int toEntityId)
        {
            return _simulatedEntities.TryGetEntityId(this.Type, entityId, to, out toEntityId);
        }

        public int GetEntityId(int entityId, SimulationType to)
        {
            return _simulatedEntities.GetEntityId(this.Type, entityId, to);
        }

        public bool GetEntity(SimulatedId id, [MaybeNullWhen(false)] out Entity entity)
        {
            if (_simulatedEntities.TryGetEntityId(id, this.Type, out var entityId))
            {
                entity = _world.GetEntity(entityId);
                return true;
            }

            entity = null;
            return false;
        }

        public Entity GetEntity(SimulatedId id)
        {
            if (_simulatedEntities.TryGetEntityId(id, this.Type, out var entityId))
            {
                return _world.GetEntity(entityId);
            }

            var entity = this.CreateEntity(id);

            return entity;
        }

        public void RemoveEntity(SimulatedId id)
        {
            _simulatedEntities.Remove(id, this.Type);
        }

        public SimulatedId GetId(int entityId)
        {
            return _simulatedEntities.GetId(this.Type, entityId);
        }

        protected abstract void Update(GameTime gameTime);

        void ISimulation.Update(GameTime gameTime)
        {
            this.Update(gameTime);
        }

        public virtual Entity CreateEntity(SimulatedId id)
        {
            var entity = _world.CreateEntity();
            _simulatedEntities.Set(id, this.Type, entity.Id);

            return entity;
        }

        private sealed class SimulationEvent<TData> : Message<ISimulationEvent<TData>>, ISimulationEvent<TData>
            where TData : notnull, ISimulationData
        {
            public PeerType Source { get; }

            public TData Data { get; }

            public ISimulation Simulation { get; }

            object ISimulationEvent.Data => this.Data;


            public SimulationEvent(PeerType source, TData data, ISimulation simulation)
            {
                this.Source = source;
                this.Data = data;
                this.Simulation = simulation;
            }
        }

        public virtual void PublishEvent(PeerType source, ISimulationData data)
        {
            _bus.Publish(GetSimulationEvent(source, data, this));
        }

        private static Dictionary<Type, Func<PeerType, ISimulationData, ISimulation, ISimulationEvent>> _eventFactories = new();
        private static MethodInfo _eventFactoryMethod = typeof(Simulation).GetMethod(nameof(SimulationEventFactory), BindingFlags.Static | BindingFlags.NonPublic) ?? throw new UnreachableException();

        private static ISimulationEvent GetSimulationEvent(PeerType source, ISimulationData data, ISimulation simulation)
        {
            var type = data.GetType();
            if(!_eventFactories.TryGetValue(type, out var factory))
            {
                var method = _eventFactoryMethod.MakeGenericMethod(type);
                factory = (Func<PeerType, ISimulationData, ISimulation, ISimulationEvent>)(method.Invoke(null, Array.Empty<object>()) ?? throw new UnreachableException());

                _eventFactories.Add(type, factory);
            }

            return factory(source, data, simulation);
        }

        private static Func<PeerType, ISimulationData, ISimulation, ISimulationEvent> SimulationEventFactory<TData>()
            where TData : ISimulationData
        {
            ISimulationEvent Factory(PeerType source, ISimulationData data, ISimulation simulation)
            {
                return new SimulationEvent<TData>(source, (TData)data, simulation);
            }

            return Factory;
        }
    }
}
