using Guppy.Tests.Common;
using Guppy.Tests.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Tests.Common.Entities.Builders
{
    public class ComponentSerializerServiceBuilder : Builder<ComponentSerializerService>
    {
        public required List<Func<IComponentSerializer>> ComponentSerializers { get; init; }

        protected override ComponentSerializerService Build()
        {
            return new ComponentSerializerService(this.ComponentSerializers.ToLazy(x => x()));
        }
    }
}
