using Guppy.Attributes;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.ShipParts;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    [AutoLoad]
    internal sealed class ResourceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddResourcePack(ResourcePacks.ShipParts.Name, ResourcePacks.ShipParts.Path);

            // services.AddColorResource(ShipPartColors.Default);
        }
    }
}
