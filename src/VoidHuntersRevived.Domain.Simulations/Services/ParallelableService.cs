using Guppy.Common.Helpers;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    internal sealed partial class ParallelableService : IParallelableService
    {
        private Dictionary<ParallelKey, Parallelable> _parallelables = new Dictionary<ParallelKey, Parallelable>();

        public Parallelable Get(ParallelKey key)
        {
            if(!_parallelables.TryGetValue(key, out Parallelable? parallelable))
            {
                parallelable = new Parallelable(key);
                _parallelables.Add(key, parallelable);
            }

            return parallelable;
        }

        public Parallelable Get(int id)
        {
            throw new NotImplementedException();
        }
    }
}
