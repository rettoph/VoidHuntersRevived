using Guppy.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Common.Pieces.Loaders
{
    [Service<IPieceLoader>(ServiceLifetime.Singleton, true)]
    public interface IPieceLoader
    {
        void Configure(IPieceConfigurationService pieces);
    }
}
