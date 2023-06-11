using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Simulations.Services
{
    public interface IEntityTypeService
    {
        void Configure(EntityType type, Action<IEntityTypeConfiguration> configuration);
    }
}
