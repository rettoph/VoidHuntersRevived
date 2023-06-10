using Guppy.Common;
using Guppy.Common.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.ECS;
using VoidHuntersRevived.Common.ECS.Factories;
using VoidHuntersRevived.Common.ECS.Systems;
using VoidHuntersRevived.Domain.ECS.Services;

namespace VoidHuntersRevived.Domain.ECS.Factories
{
    internal sealed class WorldFactory : IWorldFactory
    {
        private readonly EntityTypeService _types;
        private readonly IFilteredProvider _filtered;

        public WorldFactory(EntityTypeService types, IFilteredProvider filtered)
        {
            _types = types;
            _filtered = filtered;
        }

        public IWorld Create(params IState[] states)
        {
            return new World(
                types: _types,
                systems: _filtered.Instances<ISystem>(states).Sort().ToArray());
        }
    }
}
