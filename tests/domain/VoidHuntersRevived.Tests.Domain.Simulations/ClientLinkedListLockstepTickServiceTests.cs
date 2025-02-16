using VoidHuntersRevived.Tests.Common.Simulations.Mockers;
using VoidHuntersRevived.Tests.Domain.Simulations.Extensions;

namespace VoidHuntersRevived.Tests.Domain.Simulations
{
    public class ClientLinkedListLockstepTickServiceTests
    {
        [Fact]
        public void SingleMissingTickInLinkedListPassedIntoPrevious_ParentIsReturned()
        {
            ClientLinkedListLockstepTickServiceMocker.Create(3).PopulateEmptyTicksAndVerifyPrevious(
                tickIds: [0, 1, 3, 4],
                previousId: 2,
                expectedResult: 1);
        }

        [Fact]
        public void ManyMissingTicksInLinkedListLastMissingPassedIntoPrevious_LastContainedIsReturned()
        {
            ClientLinkedListLockstepTickServiceMocker.Create(3).PopulateEmptyTicksAndVerifyPrevious(
                tickIds: [0, 1, 7, 8],
                previousId: 6,
                expectedResult: 1);
        }

        [Fact]
        public void NoMissingTicksInLinkedListContainedPassedIntoPrevious_ParentIsReturned()
        {
            ClientLinkedListLockstepTickServiceMocker.Create(3).PopulateEmptyTicksAndVerifyPrevious(
                tickIds: [0, 1, 2, 3, 4, 5, 6],
                previousId: 3,
                expectedResult: 2);
        }
    }
}
