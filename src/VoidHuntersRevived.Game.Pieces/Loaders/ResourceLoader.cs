using Guppy.Attributes;
using Guppy.Resources;
using Guppy.Resources.Loaders;
using Guppy.Resources.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;

namespace VoidHuntersRevived.Game.Pieces.Loaders
{
    [AutoLoad]
    internal sealed class ResourceLoader : IResourceLoader
    {
        public void Load(IResourceProvider resources)
        {
            resources.Register(Piece.Resource);
        }
    }
}
