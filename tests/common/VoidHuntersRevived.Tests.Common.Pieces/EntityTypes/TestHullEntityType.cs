using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common.EntityTypes;

namespace VoidHuntersRevived.Tests.Common.Pieces.EntityTypes
{
    public abstract class TestHullEntityType : HullEntityType
    {
        public TestHullEntityType(string name) : base(Key<IEntityType>.GetByName(name))
        {
        }

        public class TestHullSquareEntityType : TestHullEntityType
        {
            public static TestHullSquareEntityType Instance = new();

            public TestHullSquareEntityType() : base(nameof(TestHullSquareEntityType))
            {
                this.WithComponents([

                ]);
            }
        }
    }
}
