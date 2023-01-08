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
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Common.Simulations
{
    public abstract class Simulation<TEntityComponent> : ISimulation
        where TEntityComponent : class, new()
    {
        private IBus _bus;
        private World _world;
        private ISimulationUpdateSystem[] _updateSystems;
        private readonly IParallelService _simulatedEntities;
        private readonly TEntityComponent _entityComponent;

        public readonly SimulationType Type;
        public readonly Aether Aether;
        public readonly Type EntityComponentType;

        SimulationType ISimulation.Type => this.Type;
        Aether ISimulation.Aether => this.Aether;
        Type ISimulation.EntityComponentType => this.EntityComponentType;

        protected Simulation(SimulationType type, IParallelService simulatedEntities)
        {
            _simulatedEntities = simulatedEntities;
            _world = default!;
            _bus = default!;
            _updateSystems = Array.Empty<ISimulationUpdateSystem>();
            _entityComponent = new TEntityComponent();

            this.Type = type;
            this.Aether = new Aether(Vector2.Zero);
            this.EntityComponentType = typeof(TEntityComponent);
        }

        public virtual void Initialize(IServiceProvider provider)
        {
            _world = provider.GetRequiredService<World>();
            _bus = provider.GetRequiredService<IBus>();
            _updateSystems = provider.GetRequiredService<IFiltered<ISimulationUpdateSystem>>().Instances.ToArray();
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
            return _simulatedEntities.TryGetEntityIdFromKey(key, this.Type, out id);
        }

        public int GetEntityId(ParallelKey key)
        {
            if (_simulatedEntities.TryGetEntityIdFromKey(key, this.Type, out var id))
            {
                return id;
            }

            var entity = this.CreateEntity(key);

            return entity.Id;
        }

        public bool TryGetEntityId(int id, SimulationType toType, [MaybeNullWhen(false)] out int toId)
        {
            return _simulatedEntities.TryGetEntityId(id, toType, out toId);
        }

        public int GetEntityId(int id, SimulationType to)
        {
            return _simulatedEntities.GetId(id, to);
        }

        public bool TryGetEntity(ParallelKey key, [MaybeNullWhen(false)] out Entity entity)
        {
            if (_simulatedEntities.TryGetEntityIdFromKey(key, this.Type, out var id))
            {
                entity = _world.GetEntity(id);
                return true;
            }

            entity = null;
            return false;
        }

        public Entity GetEntity(ParallelKey key)
        {
            if (_simulatedEntities.TryGetEntityIdFromKey(key, this.Type, out var entityId))
            {
                return _world.GetEntity(entityId);
            }

            var entity = this.CreateEntity(key);

            return entity;
        }

        public void RemoveEntity(int id)
        {
            _simulatedEntities.Remove(this.Type, id);
        }

        protected abstract void Update(GameTime gameTime);

        void ISimulation.Update(GameTime gameTime)
        {
            this.Update(gameTime);
        }

        public virtual Entity CreateEntity(ParallelKey key)
        {
            var entity = _world.CreateEntity();
            entity.Attach(_entityComponent);
            entity.Attach<ISimulation>(this);

            _simulatedEntities.Set(key, this.Type, entity.Id);

            return entity;
        }

        private sealed class SimulationInput<TData> : Message<ISimulationInput<TData>>, ISimulationInput<TData>
            where TData : notnull, ISimulationInputData
        {
            public Confidence Confidence { get; }

            public TData Data { get; }

            public ISimulation Simulation { get; }

            ISimulationInputData ISimulationInput.Data => this.Data;


            public SimulationInput(Confidence confidence, TData data, ISimulation simulation)
            {
                this.Confidence = confidence;
                this.Data = data;
                this.Simulation = simulation;
            }
        }

        public virtual void Input(ISimulationInputData data, Confidence confidence)
        {
            _bus.Publish(EventFactory.GetSimulationEvent(confidence, data, this));
        }

        private static class EventFactory
        {
            private static Dictionary<Type, Func<Confidence, ISimulationInputData, ISimulation, ISimulationInput>> _eventFactories = new();
            private static MethodInfo _eventFactoryMethod = typeof(EventFactory).GetMethod(nameof(SimulationEventFactory), BindingFlags.Static | BindingFlags.NonPublic) ?? throw new UnreachableException();

            public static ISimulationInput GetSimulationEvent(Confidence confidence, ISimulationInputData data, ISimulation simulation)
            {
                var type = data.GetType();
                if (!_eventFactories.TryGetValue(type, out var factory))
                {
                    var method = _eventFactoryMethod.MakeGenericMethod(type);
                    factory = (Func<Confidence, ISimulationInputData, ISimulation, ISimulationInput>)(method.Invoke(null, Array.Empty<object>()) ?? throw new UnreachableException());

                    _eventFactories.Add(type, factory);
                }

                return factory(confidence, data, simulation);
            }

            private static Func<Confidence, ISimulationInputData, ISimulation, ISimulationInput> SimulationEventFactory<TData>()
                where TData : ISimulationInputData
            {
                ISimulationInput Factory(Confidence confidence, ISimulationInputData data, ISimulation simulation)
                {
                    return new SimulationInput<TData>(confidence, (TData)data, simulation);
                }

                return Factory;
            }
        }
    }
}
