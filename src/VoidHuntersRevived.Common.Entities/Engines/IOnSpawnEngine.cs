using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Engines
{
    public interface IOnSpawnEngine<T>
        where T : unmanaged, IEntityComponent
    {
        void OnSpawn(EntityId id, ref T component, in GroupIndex groupIndex);
    }
}
