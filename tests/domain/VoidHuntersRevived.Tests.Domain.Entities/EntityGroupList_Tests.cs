using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Tests.Domain.Entities.Components;

namespace VoidHuntersRevived.Tests.Domain.Entities
{
    public class EntityGroupList_Tests
    {
        private struct TestComponentTwo : IEntityComponent
        {

        }

        [Fact]
        public void EntityGroupList_AutoAddExistingGroup()
        {
            EntityGroup group = EntityGroup.Create("test", [typeof(TestComponent)]);
            EntityGroupList groups = EntityGroupList.GetOrCreate([typeof(TestComponent)]);
            Assert.Equal(1, groups.Values.count);
        }

        [Fact]
        public void EntityGroupList_EnsureMatchingComponentsReused()
        {
            EntityGroup group = EntityGroup.Create("test", [typeof(TestComponent), typeof(TestComponent)]);

            EntityGroupList groups1 = EntityGroupList.GetOrCreate([typeof(TestComponentTwo), typeof(TestComponent)]);
            EntityGroupList groups2 = EntityGroupList.GetOrCreate([typeof(TestComponent), typeof(TestComponentTwo)]);

            bool refsEqual = object.ReferenceEquals(groups1, groups2);
            Assert.True(refsEqual);
        }
    }
}
