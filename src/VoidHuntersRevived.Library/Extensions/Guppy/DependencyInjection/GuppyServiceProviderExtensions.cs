using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Extensions.Guppy.DependencyInjection
{
    public static class GuppyServiceProviderExtensions
    {
        public static ShipPartService ShipParts(this GuppyServiceProvider provider)
            => provider.GetService<ShipPartService>();

        public static ChainService Chains(this GuppyServiceProvider provider)
            => provider.GetService<ChainService>();

        public static ShipService Ships(this GuppyServiceProvider provider)
            => provider.GetService<ShipService>();

        public static PlayerService Players(this GuppyServiceProvider provider)
            => provider.GetService<PlayerService>();
    }
}
