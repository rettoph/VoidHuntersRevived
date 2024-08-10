using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Tests.Domain.Entities.Components;

namespace VoidHuntersRevived.Tests.Domain.Entities.Descriptors
{
    public class TestEntityType : EntityType
    {
        public static readonly IKey<TestEntityType> TestEntityTypeKey = VoidHuntersRevived.Common.Key.GetByName<TestEntityType>(nameof(TestEntityType));

        public TestEntityType() : base(TestEntityTypeKey, Array.Empty<IKey<IEntityType>>())
        {
            this.WithInstanceEntityComponents([
                new TestComponent()
            ]);
        }
    }
}
