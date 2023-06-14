using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Game.Pieces.Properties;

namespace VoidHuntersRevived.Game.Pieces.Loaders
{
    [AutoLoad]
    internal sealed class PieceTypeLoader : IEntityTypeLoader
    {
        public void Configure(IEntityTypeService entityTypes)
        {
            entityTypes.Configure(PieceTypes.Hull, configuration =>
            {
                configuration.HasProperty<Rigid>()
                    .HasComponent<Node>();
            });
        }
    }
}
