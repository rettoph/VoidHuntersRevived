using Guppy.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.ECS.Services;

namespace VoidHuntersRevived.Common.ECS.Loaders
{
    [Service<IEntityTypeLoader>(ServiceLifetime.Singleton, true)]
    public interface IEntityTypeLoader
    {
        void Configure(IEntityTypeService entityTypes);
    }
}
