using Guppy.Attributes;
using Guppy.Common.DependencyInjection;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Physics.Factories;
using VoidHuntersRevived.Domain.Physics.Factories;

namespace VoidHuntersRevived.Domain.Physics.Loaders
{
    [AutoLoad]
    public sealed class PhysicsLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IBodyFactory, BodyFactory>();
            services.AddScoped<ISpace, Space>();
        }
    }
}
