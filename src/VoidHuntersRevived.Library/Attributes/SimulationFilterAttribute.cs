using Guppy.Common.Attributes;
using Guppy.Network.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Filters;
using VoidHuntersRevived.Library.Simulations;

namespace VoidHuntersRevived.Library.Attributes
{
    public sealed class SimulationFilterAttribute : InitializableAttribute
    {
        public readonly SimulationType Simulation;

        public SimulationFilterAttribute(SimulationType simulation)
        {
            this.Simulation = simulation;
        }

        public override void Initialize(IServiceCollection services, Type classType)
        {
            base.Initialize(services, classType);

            services.AddFilter(new SimulationFilter(this.Simulation, classType));
        }
    }
}
