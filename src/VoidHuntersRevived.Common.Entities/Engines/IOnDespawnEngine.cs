using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Engines
{
    public interface IOnDespawnEngine<T>
        where T : unmanaged, IEntityComponent
    {
        void OnDespawn(EntityId id, ref T component, in GroupIndex groupIndex);
    }
}
