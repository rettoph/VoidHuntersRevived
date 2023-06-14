using Guppy.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Entities.Loaders
{
    [Service<IEntityLoader>(ServiceLifetime.Scoped, true)]
    public interface IEntityLoader
    {
        void Configure(IEntityConfigurationService configuration);
    }
}
