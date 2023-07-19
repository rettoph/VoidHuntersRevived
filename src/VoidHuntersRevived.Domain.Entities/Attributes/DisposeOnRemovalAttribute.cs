using Autofac;
using Guppy.Attributes;
using Guppy.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Entities.Engines;

namespace VoidHuntersRevived.Domain.Entities.Attributes
{
    public class DisposeOnRemovalAttribute : GuppyConfigurationAttribute
    {
        protected override void Configure(GuppyConfiguration configuration, Type classType)
        {
            configuration.Builder.RegisterGeneric(typeof(DisposableEngine<>).MakeGenericType(classType)).InstancePerLifetimeScope();
        }
    }
}
