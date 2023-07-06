using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IFilterService
    {
        ref EntityFilterCollection GetFilter<T>(VhId filterVhId)
             where T : unmanaged, IEntityComponent;
    }
}
