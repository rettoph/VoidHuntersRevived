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
using VoidHuntersRevived.Common.Simulations.Providers;
using Volatile;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulation
    {
        SimulationType Type { get; }
        Aether Aether { get; }
        Type EntityComponentType { get; }
        IServiceProvider Provider { get; }
        IParallelKeyProvider Keys { get; }

        void Initialize(IServiceProvider provider);

        bool TryGetEntityId(ParallelKey key, [MaybeNullWhen(false)] out int id);

        int GetEntityId(ParallelKey key);

        bool HasEntity(ParallelKey key);

        void Update(GameTime gameTime);

        int CreateEntity(ParallelKey key, EntityType type);

        int CreateEntity(ParallelKey key, EntityType type, Action<Entity> factory);

        void DestroyEntity(ParallelKey key);

        void ConfigureEntity(Entity entity);

        ISimulationEvent Publish(SimulationEventData data);

        void Enqueue(SimulationEventData data);
    }
}
