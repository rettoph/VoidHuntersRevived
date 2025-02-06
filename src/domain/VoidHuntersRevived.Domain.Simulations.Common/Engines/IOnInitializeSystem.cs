using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common.Systems
{
    public interface IOnInitializeSystem<in TStrategy>
        where TStrategy : IStrategy
    {
        [RequireSequenceGroup<OnInitializeSequenceGroupEnum>]
        void OnInitialize(TStrategy strategy);
    }

    public interface IOnInitializeEngine : IOnInitializeSystem<IStrategy>
    {

    }
}