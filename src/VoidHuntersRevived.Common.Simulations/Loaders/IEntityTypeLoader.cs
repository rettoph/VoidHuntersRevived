using Guppy.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Common.Simulations.Loaders
{
    [Service<IEntityTypeLoader>(ServiceLifetime.Singleton, true)]
    public interface IEntityTypeLoader
    {
        void Configure(IEntityTypeService entityTypes);
    }
}
