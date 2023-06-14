using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Common.Entities
{
    public delegate void InitializeComponentDelegate<T>(T property, int id, ref EntityInitializer initializer)
        where T : class, IEntityProperty;

    public interface IEntityPropertyConfiguration
    {
        Type Type { get; }
    }

    public interface IEntityPropertyConfiguration<T> : IEntityPropertyConfiguration
        where T : class, IEntityProperty
    {
        void HasComponent<TComponent>()
            where TComponent : unmanaged, IEntityComponent;

        void HasInitializer(InitializeComponentDelegate<T> initializer);
    }
}
