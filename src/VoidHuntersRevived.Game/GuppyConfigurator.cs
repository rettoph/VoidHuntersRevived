using Guppy;
using Guppy.Attributes;
using Guppy.Common.Providers;
using Guppy.Configurations;
using Guppy.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Physics.Loaders;
using VoidHuntersRevived.Domain.Loaders;
using VoidHuntersRevived.Game.Pieces.Loaders;
using VoidHuntersRevived.Game.Ships.Loaders;
using VoidHuntersRevived.Game.Common.Loaders;

namespace VoidHuntersRevived.Domain
{
    [AutoLoad]
    internal sealed class GuppyConfigurator : IGuppyConfigurator
    {
        public void Configure(GuppyConfiguration configuration)
        {
            configuration.Assemblies.Load(typeof(DomainLoader).Assembly);
            configuration.Assemblies.Load(typeof(EntityTypeLoader).Assembly);
            configuration.Assemblies.Load(typeof(PieceTypeLoader).Assembly);
            configuration.Assemblies.Load(typeof(ShipsServiceLoader).Assembly);
        }
    }
}
