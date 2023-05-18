using Guppy.Common.Helpers;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    internal sealed partial class ParallelableService : IParallelableService
    {
        private Dictionary<ParallelKey, Parallelable> _parallelables = new Dictionary<ParallelKey, Parallelable>();
        private Dictionary<int, Parallelable> _ids = new Dictionary<int, Parallelable>();

        public Parallelable Get(ParallelKey key)
        {
            if(!_parallelables.TryGetValue(key, out Parallelable? parallelable))
            {
                parallelable = new Parallelable(key, this.HandleAdded, this.HandleRemoved, this.HandleEmpty);
                _parallelables.Add(key, parallelable);
            }

            return parallelable;
        }

        public Parallelable Get(int id)
        {
            return _ids[id];
        }

        public bool TryGet(int id, [MaybeNullWhen(false)] out Parallelable parallelable)
        {
            return _ids.TryGetValue(id, out parallelable);
        }

        private void HandleAdded(Parallelable sender, int args)
        {
            _ids.Add(args, sender);
        }

        private void HandleRemoved(Parallelable sender, int args)
        {
            _ids.Remove(args);
        }

        private void HandleEmpty(Parallelable args)
        {
            _parallelables.Remove(args.Key);
        }
    }
}
