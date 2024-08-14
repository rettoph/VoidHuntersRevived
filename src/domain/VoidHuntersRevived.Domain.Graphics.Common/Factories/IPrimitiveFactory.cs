using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using VoidHuntersRevived.Domain.Graphics.Common;

namespace VoidHuntersRevived.Domain.Graphics.Factories
{
    [Service<IPrimitiveFactory>(ServiceLifetime.Scoped, ServiceRegistrationFlags.RequireAutoLoadAttribute)]
    public interface IPrimitiveFactory
    {
        IEnumerable<IPrimitive> BuildPrimitives();
    }
}
