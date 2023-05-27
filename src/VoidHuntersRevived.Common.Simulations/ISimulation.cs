using Guppy.ECS;
using Guppy.Network;
using Guppy.Network.Identity;
using LiteNetLib;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Physics;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulation
    {
        SimulationType Type { get; }
        ISpace Space { get; }
        Type EntityComponentType { get; }
        IServiceProvider Provider { get; }

        void Initialize(IServiceProvider provider);

        void Update(GameTime gameTime);

        int CreateEntity(ParallelKey key, EntityType type);

        int CreateEntity(ParallelKey key, EntityType type, Action<Entity> factory);

        void DestroyEntity(ParallelKey key);

        ISimulationEvent Publish(SimulationEventData data);

        void Enqueue(SimulationEventData data);

        bool TryGetEntityId(ParallelKey key, [MaybeNullWhen(false)] out int entityId);

        int GetEntityId(ParallelKey key);

        bool HasEntity(ParallelKey key);

        void Configure(Entity entity);

        void Map(ParallelKey entityKey, int entityId);

        void Unmap(ParallelKey entityKey);
    }
}
