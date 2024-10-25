using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Tests.Domain.Entities.Components;

namespace VoidHuntersRevived.Tests.Domain.Entities.EntityTypes
{
    public class TestEntityTemplate : EntityTemplate
    {
        public static readonly Key<EntityTemplate> TestEntityTypeKey = Key<EntityTemplate>.GetByName(nameof(TestEntityTemplate));

        public TestEntityTemplate() : base()
        {
            this.Components = [
                new TestComponent()
            ];
        }
    }
}
