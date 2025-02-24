using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Tests.Entities.Stubs
{
    public struct TestEntityComponentTwo : IEntityComponent
    {
        public static readonly Key<IEntityTemplate> TestEntityTemplateKey = Key<IEntityTemplate>.GetByName("TestEntityTemplate");
        public static readonly EntityTemplateFragment TestEntityTemplateFragment = new()
        {
            Key = TestEntityTemplateKey,
            Components = [
                new TestEntityComponentTwo()
            ]
        };

        public int Value;
    }
}
