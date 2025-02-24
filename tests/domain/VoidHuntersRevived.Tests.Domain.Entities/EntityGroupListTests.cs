using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Tests.Common.Entities.Stubs;

namespace VoidHuntersRevived.Tests.Domain.Entities
{
    [Collection("EntityGroups")]
    public class EntityGroupListTests
    {
        [Fact]
        public void CreatedEntityGroup_IsAddedToEntityGroupList()
        {
            // Reset
            EntityGroup.Clear();
            EntityGroupList.Clear();
            Assert.Empty(EntityGroup.GetAll());
            Assert.Empty(EntityGroupList.GetAll());

            // Create group list and ensure list is empty
            EntityGroupList groups = EntityGroupList.GetOrCreate([typeof(TestEntityComponent)]);
            Assert.Equal(0, groups.Values.count);

            // Create group and ensure list has been updated
            _ = EntityGroup.Create("test", [typeof(TestEntityComponent)]);
            Assert.Equal(1, groups.Values.count);
        }

        [Fact]
        public void CreatedGroupListsWithUnorderedMatchingComponents_AreEqual()
        {
            EntityGroup.Clear();
            Assert.Empty(EntityGroup.GetAll());

            EntityGroupList.Clear();
            Assert.Empty(EntityGroupList.GetAll());
            _ = EntityGroup.Create("test", [typeof(TestEntityComponent), typeof(TestEntityComponent)]);

            EntityGroupList groups1 = EntityGroupList.GetOrCreate([typeof(TestEntityComponentTwo), typeof(TestEntityComponent)]);
            EntityGroupList groups2 = EntityGroupList.GetOrCreate([typeof(TestEntityComponent), typeof(TestEntityComponentTwo)]);

            bool refsEqual = object.ReferenceEquals(groups1, groups2);
            Assert.True(refsEqual);
        }
    }
}