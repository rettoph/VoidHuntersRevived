using Guppy.Network.Enums;
using LiteNetLib;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common
{
    public interface ISimulation
    {
        SimulationType Type { get; }
        Aether Aether { get; }

        void Initialize(IServiceProvider provider);

        bool TryGetEntityId(SimulatedId id, [MaybeNullWhen(false)] out int entityId);

        int GetEntityId(SimulatedId id);

        bool TryGetEntityId(int entityId, SimulationType to, [MaybeNullWhen(false)] out int toEntityId);

        int GetEntityId(int entityId, SimulationType to);

        bool GetEntity(SimulatedId id, [MaybeNullWhen(false)] out Entity entity);

        Entity GetEntity(SimulatedId id);

        void RemoveEntity(SimulatedId id);

        SimulatedId GetId(int entityId);

        void Update(GameTime gameTime);

        Entity CreateEntity(SimulatedId id);

        void PublishEvent(PeerType source, ISimulationData data);
    }
}
