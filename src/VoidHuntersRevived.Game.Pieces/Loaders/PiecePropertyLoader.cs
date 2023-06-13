using Guppy.Attributes;
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
    internal sealed class PiecePropertyLoader : IPiecePropertyLoader
    {
        public void Configure(IPiecePropertyService properties)
        {
        }
    }
}
