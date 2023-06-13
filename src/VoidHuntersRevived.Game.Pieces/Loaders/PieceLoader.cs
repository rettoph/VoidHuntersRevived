using FixedMath.NET;
using Guppy.Attributes;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces.Loaders;
using VoidHuntersRevived.Common.Pieces.Services;
using VoidHuntersRevived.Game.Pieces.Properties;

namespace VoidHuntersRevived.Game.Pieces.Loaders
{
    [AutoLoad]
    public class PieceLoader : IPieceLoader
    {
        public void Configure(IPieceConfigurationService pieces)
        {
            pieces.Configure(PieceTypes.HullSquare, configuration =>
            {
                configuration.SetCategory(PieceCategories.Hull)
                    .AddProperty(Rigid.Polygon(Fix64.One, 4));
            });
        }
    }
}
