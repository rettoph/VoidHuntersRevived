using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Tests.Entities.Stubs
{
    public struct TestEntityComponent : IEntityComponent
    {
        public static readonly Key<IEntityTemplate> TestEntityTemplateKey = Key<IEntityTemplate>.GetByName("TestEntityTemplate");
        public static readonly EntityTemplateFragment TestEntityTemplateFragment = new()
        {
            Key = TestEntityTemplateKey,
            Components = [
                new TestEntityComponent()
            ]
        };

        public int Value;
    }
}
