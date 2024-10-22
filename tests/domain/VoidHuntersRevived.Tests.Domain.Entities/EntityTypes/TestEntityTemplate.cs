using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Tests.Domain.Entities.Components;

namespace VoidHuntersRevived.Tests.Domain.Entities.EntityTypes
{
    public class TestEntityTemplate : BaseEntityTemplate
    {
        public static readonly Key<IEntityTemplate> TestEntityTypeKey = Key<IEntityTemplate>.GetByName(nameof(TestEntityTemplate));

        public TestEntityTemplate() : base(TestEntityTypeKey)
        {
            this.WithComponents([
                new TestComponent()
            ]);
        }
    }
}
