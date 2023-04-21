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

        bool TryGetEntityId(ParallelKey key, [MaybeNullWhen(false)] out int id);

        int GetEntityId(ParallelKey key);

        bool HasEntity(ParallelKey key);

        void DestroyEntity(ParallelKey key);

        void Update(GameTime gameTime);

        Entity CreateEntity(ParallelKey key);

        void PublishEvent(IData data);

        void Input(ParallelKey user, IData data);
    }
}
