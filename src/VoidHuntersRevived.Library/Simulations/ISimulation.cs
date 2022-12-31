using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Simulations
{
    public interface ISimulation
    {
        SimulationType Type { get; }
        AetherWorld Aether { get; }

        void Initialize(IServiceProvider provider);

        internal void Update(GameTime gameTime);

        bool TryGetEntityId(SimulatedId id, [MaybeNullWhen(false)] out int entityId);
        int GetEntityId(SimulatedId id);
        bool TryGetEntityId(int entityId, SimulationType to, [MaybeNullWhen(false)] out int toEntityId);
        int GetEntityId(int entityId, SimulationType to);
        bool GetEntity(SimulatedId id, [MaybeNullWhen(false)] out Entity entity);
        Entity GetEntity(SimulatedId id);
        void RemoveEntity(SimulatedId id);

        SimulatedId GetId(int entityId);

        Entity CreateEntity();
    }
}
