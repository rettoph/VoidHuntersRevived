using Autofac;
using Guppy;
using Guppy.Attributes;
using Guppy.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Descriptors;

namespace VoidHuntersRevived.Domain.Entities.Configurators
{
    [AutoLoad]
    internal sealed class VoidHuntersEntityDescriptorConfigurator : IGuppyConfigurator
    {
        public void Configure(GuppyConfiguration configuration)
        {
            var descriptors = configuration.Assemblies.GetTypes<VoidHuntersEntityDescriptor>().Where(x => x.IsClass && !x.IsAbstract && !x.IsGenericType).ToArray();
            foreach (Type descriptorType in descriptors)
            {
                configuration.Builder.RegisterType(descriptorType).As<VoidHuntersEntityDescriptor>().SingleInstance();
            }
        }
    }
}
