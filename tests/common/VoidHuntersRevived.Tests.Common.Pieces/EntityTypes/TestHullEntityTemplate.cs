using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common.EntityTemplates;

namespace VoidHuntersRevived.Tests.Common.Pieces.EntityTypes
{
    public abstract class TestHullEntityTemplate : HullEntityTemplate
    {
        public TestHullEntityTemplate(string name) : base(Key<EntityTemplate>.GetByName(name))
        {
        }

        public class TestHullSquareEntityTemplate : TestHullEntityTemplate
        {
            public static TestHullSquareEntityTemplate Instance = new();

            public TestHullSquareEntityTemplate() : base(nameof(TestHullSquareEntityTemplate))
            {
                this.WithComponents([

                ]);
            }
        }
    }
}
