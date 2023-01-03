using Guppy.Common.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Filters;

namespace VoidHuntersRevived.Common.Attributes
{
    public sealed class SimulationTypeFilterAttribute : InitializableAttribute
    {
        public readonly SimulationType SimulationType;

        public SimulationTypeFilterAttribute(SimulationType simulationType)
        {
            this.SimulationType = simulationType;
        }

        public override void Initialize(IServiceCollection services, Type classType)
        {
            base.Initialize(services, classType);

            services.AddFilter(new SimulationTypeFilter(this.SimulationType, classType));
        }
    }
}
