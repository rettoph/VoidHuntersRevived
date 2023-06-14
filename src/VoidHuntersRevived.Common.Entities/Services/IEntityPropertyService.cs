using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityPropertyService
    {
        void Configure<T>(Action<IEntityPropertyConfiguration<T>> configuration)
            where T : class, IEntityProperty;
    }
}
