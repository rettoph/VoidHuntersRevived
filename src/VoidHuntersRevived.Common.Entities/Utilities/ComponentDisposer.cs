using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Utilities
{
    public abstract class ComponentDisposer
    {
        internal ComponentDisposer()
        {

        }

        public abstract void Dispose(uint index, ExclusiveGroupStruct group, EntitiesDB entities);

        public static ComponentDisposer Build(Type disposable)
        {
            Type type = typeof(ComponentDisposer<>).MakeGenericType(disposable);
            return (ComponentDisposer)Activator.CreateInstance(type)!;
        }
    }

    internal sealed class ComponentDisposer<T> : ComponentDisposer
        where T : unmanaged, IEntityComponent, IDisposable
    {
        public override void Dispose(uint index, ExclusiveGroupStruct groupID, EntitiesDB entities)
        {
            var (disposables, _) = entities.QueryEntities<T>(groupID);

            disposables[index].Dispose();
        }
    }
}
