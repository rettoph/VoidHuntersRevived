using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Common.Providers
{
    [StrategyFilter(StrategyTypeEnum.Predictive)]
    [Service<IPrimitiveProvider>(ServiceLifetime.Scoped, ServiceRegistrationFlags.RequireAutoLoadAttribute)]
    public interface IPrimitiveProvider
    {
        IEnumerable<IPrimitive> GetPrimitives();
    }
}
