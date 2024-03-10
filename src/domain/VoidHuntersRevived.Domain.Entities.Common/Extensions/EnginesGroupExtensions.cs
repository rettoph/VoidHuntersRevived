using Guppy.Common.Extensions;
using Svelto.DataStructures;
using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Extensions
{
    public static class EnginesGroupExtensions
    {
        public static IStepGroupEngine<T> CreateSequencedStepEnginesGroup<T, TSequence>(
            this IEnumerable<IEngine> engines,
            TSequence defaultSequence)
            where TSequence : unmanaged, Enum
        {
            return new SimpleEnginesGroup<IStepEngine<T>, T>(engines.OfType<IStepEngine<T>>().Sequence(defaultSequence));
        }

        public static IStepGroupEngine<T> CreateStepEnginesGroup<T>(this IEnumerable<IEngine> engines)
        {
            return new SimpleEnginesGroup<IStepEngine<T>, T>(engines.OfType<IStepEngine<T>>());
        }

        private class SimpleEnginesGroup<TInterface, TParameter> : UnsortedEnginesGroup<TInterface, TParameter>
            where TInterface : class, IStepEngine<TParameter>
        {
            public SimpleEnginesGroup(IEnumerable<TInterface> engines) : base(new FasterList<TInterface>(engines.ToArray()))
            {

            }
        }
    }
}
