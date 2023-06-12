using Guppy.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Systems
{
    [Service<ISystem>(ServiceLifetime.Transient, true)]
    public interface ISystem : IDisposable
    {
        void Initialize(ISimulation simulation);
    }
}
