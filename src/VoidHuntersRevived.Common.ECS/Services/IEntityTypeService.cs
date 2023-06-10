using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.ECS.Services
{
    public interface IEntityTypeService
    {
        void Configure(EntityType type, Action<IEntityTypeConfiguration> configuration);
    }
}
