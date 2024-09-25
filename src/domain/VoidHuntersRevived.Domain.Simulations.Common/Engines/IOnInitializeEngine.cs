using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    public interface IOnInitializeEngine<in TStrategy>
        where TStrategy : IStrategy
    {
        [RequireSequenceGroup<OnInitializeSequenceGroup>]
        void OnInitialize(TStrategy strategy);
    }

    public interface IOnInitializeEngine : IOnInitializeEngine<IStrategy>
    {

    }
}
