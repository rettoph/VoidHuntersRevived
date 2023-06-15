using FixedMath.NET;
using Guppy.Attributes;
using Guppy.Loaders;
using Microsoft.Extensions.DependencyInjection;
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
    public class PieceLoader : IEntityLoader
    {
        public void Configure(IEntityConfigurationService configuration)
        {
            configuration.Configure(PieceNames.HullTriangle, configuration =>
            {
                configuration.SetType(PieceTypes.Hull)
                    .AddProperty(Rigid.Polygon(Fix64.One, 3))
                    .AddProperty(Visible.Polygon("orange", 3));
            });

            configuration.Configure(PieceNames.HullSquare, configuration =>
            {
                configuration.SetType(PieceTypes.Hull)
                    .AddProperty(Rigid.Polygon(Fix64.One, 4))
                    .AddProperty(Visible.Polygon("orange", 4));
            });
        }
    }
}
