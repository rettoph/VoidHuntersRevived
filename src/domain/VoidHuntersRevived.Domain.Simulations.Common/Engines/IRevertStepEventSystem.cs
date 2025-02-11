using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common.Systems
{
    public interface IRevertStepEventSystem : IStepEventSystem
    {
    }

    public interface IRevertEventSystem<T> : IRevertStepEventSystem
        where T : IStepEvent
    {
        [RequireSequenceGroup<RevertEventSequenceGroupEnum>]
        void Revert(in VhId eventId, T data);
    }
}