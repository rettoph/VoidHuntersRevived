using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Abstractions
{
    [AllowMultiple]
    internal sealed class SystemEngine : IGetReadyEngine
    {
        private readonly ISystem _system;
        private readonly IWorld _world;

        public EntitiesDB entitiesDB { get; set; } = null!;

        public SystemEngine(IWorld world, ISystem system)
        {
            _system = system;
            _world = world;
        }

        public void Ready()
        {
            _system.Initialize(_world);
        }
    }
}
