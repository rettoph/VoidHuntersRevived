using Guppy.Tests.Common;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Services;
using VoidHuntersRevived.Tests.Common.Extensions;

namespace VoidHuntersRevived.Tests.Common.Entities.Builders
{
    public class ComponentSerializerServiceBuilder : Builder<ComponentSerializerService>
    {
        public required List<IComponentSerializer> ComponentSerializers { get; init; }

        protected override ComponentSerializerService Build()
        {
            return new ComponentSerializerService(this.ComponentSerializers.ToFiltered().ToLazy());
        }
    }
}
