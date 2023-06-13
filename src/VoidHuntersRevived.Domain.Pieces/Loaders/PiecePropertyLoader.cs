using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Loaders;
using VoidHuntersRevived.Common.Pieces.Properties;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Domain.Pieces.Loaders
{
    [AutoLoad]
    internal sealed class PiecePropertyLoader : IPiecePropertyLoader
    {
        public void Configure(IPiecePropertyService properties)
        {
            properties.Configure<Core>(configuration =>
            {
                configuration.RequiresComponent((PieceProperty<Core> property, ref Local local) =>
                {
                    //
                });
            });
        }
    }
}
