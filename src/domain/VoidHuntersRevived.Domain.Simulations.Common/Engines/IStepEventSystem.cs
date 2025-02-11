using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common.Systems
{
    public interface IStepEventSystem : ISceneSystem
    {
    }

    public interface IEventSystem<T> : IStepEventSystem
        where T : IStepEvent
    {
        [RequireSequenceGroup<EventSequenceGroupEnum>]
        void Process(in VhId eventId, T data);
    }
}