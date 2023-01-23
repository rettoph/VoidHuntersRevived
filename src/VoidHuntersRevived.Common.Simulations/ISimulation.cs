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

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulation
    {
        SimulationType Type { get; }
        Aether Aether { get; }
        Type EntityComponentType { get; }
        IServiceProvider Provider { get; }

        void Initialize(IServiceProvider provider);
        void PostInitialize();

        bool TryGetEntityId(ParallelKey key, [MaybeNullWhen(false)] out int id);

        int GetEntityId(ParallelKey key);

        bool TryGetEntityId(int id, SimulationType toType, [MaybeNullWhen(false)] out int toId);

        int GetEntityId(int id, SimulationType to);

        bool TryGetEntity(ParallelKey key, [MaybeNullWhen(false)] out Entity entity);

        Entity GetEntity(ParallelKey key);

        bool HasEntity(ParallelKey key);

        void AddEntity(ParallelKey key, Entity entity);
        void RemoveEntity(ParallelKey key, out Entity entity);

        void Update(GameTime gameTime);

        void PublishEvent(IData data);

        void Input(ParallelKey user, IData data);
    }
}
