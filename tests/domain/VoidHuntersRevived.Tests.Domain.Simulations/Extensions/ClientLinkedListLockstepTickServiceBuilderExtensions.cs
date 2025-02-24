using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Tests.Common.Simulations.Mockers;

namespace VoidHuntersRevived.Tests.Domain.Simulations.Extensions
{
    public static class ClientLinkedListLockstepTickServiceBuilderExtensions
    {
        public static ClientLinkedListLockstepTickServiceBuilder PopulateEmptyTicksAndVerifyPrevious(
            this ClientLinkedListLockstepTickServiceBuilder clientLinkedListLockstepTickServiceBuilder,
            int[] tickIds,
            int previousId,
            int? expectedResult)
        {
            // Populate with empty ticks
            foreach (int tickId in tickIds)
            {
                clientLinkedListLockstepTickServiceBuilder.Object.TryEnqueue(Tick.Empty(tickId));
            }

            // get previous
            Tick? previous = clientLinkedListLockstepTickServiceBuilder.Object.Previous(previousId);

            // verify previous
            Assert.Equal(expectedResult, previous?.Id);

            return clientLinkedListLockstepTickServiceBuilder;
        }
    }
}
