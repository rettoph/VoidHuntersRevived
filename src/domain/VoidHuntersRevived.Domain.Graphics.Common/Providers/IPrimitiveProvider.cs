using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;

namespace VoidHuntersRevived.Domain.Graphics.Common.Providers
{
    [Service<IPrimitiveProvider>(ServiceLifetime.Singleton, ServiceRegistrationFlags.RequireAutoLoadAttribute)]
    public interface IPrimitiveProvider
    {
        IEnumerable<IPrimitive> GetPrimitives();
    }
}
