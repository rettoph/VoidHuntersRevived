using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;

namespace VoidHuntersRevived.Domain.Entities.EnginesGroups
{
    internal abstract class OnCloneEnginesGroup
    {
        public abstract void Invoke(in IdMap sourceId, in IdMap cloneId, ref EntityInitializer cloneInitializer);
    }

    internal sealed class OnCloneEnginesGroup<T> : OnCloneEnginesGroup
        where T : struct, IEntityComponent
    {
        private FasterList<IOnCloneEngine<T>> _engines;

        public OnCloneEnginesGroup(IEnumerable<IEngine> engines)
        {
            _engines = new FasterList<IOnCloneEngine<T>>(engines.OfType<IOnCloneEngine<T>>().ToArray());
        }

        public override void Invoke(in IdMap sourceId, in IdMap cloneId, ref EntityInitializer cloneInitializer)
        {
            ref T component = ref cloneInitializer.Get<T>();
            foreach(IOnCloneEngine<T> engine in _engines)
            {
                engine.OnCloned(in sourceId, in cloneId, ref component);
            }
        }
    }
}
