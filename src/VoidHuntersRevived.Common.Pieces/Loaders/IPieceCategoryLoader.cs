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
    [Service<IPieceCategoryLoader>(ServiceLifetime.Singleton, true)]
    public interface IPieceCategoryLoader
    {
        void Configure(IPieceCategoryService categories);
    }
}
