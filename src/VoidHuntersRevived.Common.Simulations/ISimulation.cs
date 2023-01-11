using Guppy.Network;
using LiteNetLib;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulation
    {
        SimulationType Type { get; }
        Aether Aether { get; }
        Type EntityComponentType { get; }
        IServiceProvider Provider { get; }

        void Initialize(IServiceProvider provider);

        bool TryGetEntityId(ParallelKey key, [MaybeNullWhen(false)] out int id);

        int GetEntityId(ParallelKey key);

        bool TryGetEntityId(int id, SimulationType toType, [MaybeNullWhen(false)] out int toId);

        int GetEntityId(int id, SimulationType to);

        bool TryGetEntity(ParallelKey key, [MaybeNullWhen(false)] out Entity entity);

        Entity GetEntity(ParallelKey key);

        void RemoveEntity(int id);

        void Update(GameTime gameTime);

        Entity CreateEntity(ParallelKey key);

        void PublishEvent(ISimulationData data, Confidence confidence);
    }
}
