using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityResourceService<T>
        where T : IEntityResource<T>
    {
        T GetById(Id<T> id);

        IEnumerable<T> GetAll();
    }
}
