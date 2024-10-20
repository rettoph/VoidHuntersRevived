using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Tests.Domain.Entities.Components;

namespace VoidHuntersRevived.Tests.Domain.Entities.Descriptors
{
    public class TestEntityType : BaseEntityType
    {
        public static readonly Key<IEntityType> TestEntityTypeKey = Key<IEntityType>.GetByName(nameof(TestEntityType));

        public TestEntityType() : base(TestEntityTypeKey)
        {
            this.WithComponents([
                new TestComponent()
            ]);
        }
    }
}
