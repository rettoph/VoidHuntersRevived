using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Serialization;

namespace VoidHuntersRevived.Common.Simulations
{
    public interface ISimulation
    {
        SimulationType Type { get; }
        ISpace Space { get; }
        IEntityService Entities { get; }
        IEntitySerializationService Serialization { get; }
        IEngineService Engines { get; }
        IEventPublishingService Events { get; }
        IFilterService Filters { get; }

        void Initialize(ISimulationService simulations);

        void Draw(GameTime realTime);

        void Update(GameTime realTime);

        void Publish(EventDto @event);
        void Publish(VhId eventId, IEventData data);

        void Input(VhId eventId, IInputData data);

        EntityData Serialize(IdMap id);
        EntityData Serialize(VhId vhid);
        EntityData Serialize(EGID egid);
        EntityData Serialize(uint entityId, ExclusiveGroupStruct groupId);

        void Serialize(IdMap id, EntityWriter writer);
        void Serialize(VhId vhid, EntityWriter writer);
        void Serialize(EGID egid, EntityWriter writer);
        void Serialize(uint entityId, ExclusiveGroupStruct groupId, EntityWriter writer);

        IdMap Deserialize(VhId seed, EntityData data);
        IdMap Deserialize(EntityReader reader);
    }
}
