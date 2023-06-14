using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityConfigurationService
    {
        void Configure(EntityName name, Action<IEntityConfiguration> configuration);
    }
}
