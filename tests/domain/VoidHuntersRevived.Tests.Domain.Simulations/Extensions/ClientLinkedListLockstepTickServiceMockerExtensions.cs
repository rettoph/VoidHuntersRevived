using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Tests.Common.Simulations.Mockers;

namespace VoidHuntersRevived.Tests.Domain.Simulations.Extensions
{
    public static class ClientLinkedListLockstepTickServiceMockerExtensions
    {
        public static ClientLinkedListLockstepTickServiceMocker PopulateEmptyTicksAndVerifyPrevious(
            this ClientLinkedListLockstepTickServiceMocker clientLinkedListLockstepTickServiceMocker,
            int[] tickIds,
            int previousId,
            int? expectedResult)
        {
            // Populate with empty ticks
            foreach (int tickId in tickIds)
            {
                clientLinkedListLockstepTickServiceMocker.ClientLinkedListLockstepTickService.TryEnqueue(Tick.Empty(tickId));
            }

            // get previous
            Tick? previous = clientLinkedListLockstepTickServiceMocker.ClientLinkedListLockstepTickService.Previous(previousId);

            // verify previous
            Assert.Equal(expectedResult, previous?.Id);

            return clientLinkedListLockstepTickServiceMocker;
        }
    }
}
