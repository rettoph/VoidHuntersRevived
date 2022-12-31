using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Extensions;
using Guppy.Common.Implementations;
using Guppy.Common.Providers;
using Guppy.ECS.Providers;
using Microsoft.Extensions.Options;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Simulations
{
    public abstract class Simulation : ISimulation
    {
        private readonly Lazy<World> _world;
        private readonly SimulatedEntityIdService _simulatedEntities;

        public abstract SimulationType Type { get; }
        public abstract AetherWorld Aether { get; }

        protected Simulation(
            Lazy<World> world, 
            SimulatedEntityIdService simulatedEntities)
        {
            _world = world;
            _simulatedEntities = simulatedEntities;
        }

        public bool TryGetEntityId(SimulatedId id, [MaybeNullWhen(false)] out int entityId)
        {
            return _simulatedEntities.TryGetEntityId(id, this.Type, out entityId);
        }

        public bool GetEntity(SimulatedId id, [MaybeNullWhen(false)] out Entity entity)
        {
            if(_simulatedEntities.TryGetEntityId(id, this.Type, out var entityId))
            {
                entity = _world.Value.GetEntity(entityId);
                return true;
            }

            entity = null;
            return false;
        }

        public int GetEntityId(SimulatedId id)
        {
            if(_simulatedEntities.TryGetEntityId(id, this.Type, out var entityId))
            {
                return entityId;
            }

            var entity = _world.Value.CreateEntity();
            _simulatedEntities.Set(id, this.Type, entity.Id);

            return entity.Id;
        }

        public Entity GetEntity(SimulatedId id)
        {
            if (_simulatedEntities.TryGetEntityId(id, this.Type, out var entityId))
            {
                return _world.Value.GetEntity(entityId);
            }

            var entity = _world.Value.CreateEntity();
            _simulatedEntities.Set(id, this.Type, entity.Id);

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
    }
}
