using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Tests.Domain.Entities.Components;

namespace VoidHuntersRevived.Tests.Domain.Entities.Descriptors
{
    public class TestEntityDescriptor : VoidHuntersEntityDescriptor
    {
        public TestEntityDescriptor()
        {
            this.WithInstanceComponents([
                new ComponentBuilder<TestComponent>()
            ]);
        }
    }
}
