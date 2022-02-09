using Guppy.EntityComponent.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Extensions.Guppy.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        public static ShipPartService ShipParts(this ServiceProvider provider)
            => provider.GetService<ShipPartService>();

        public static ChainService Chains(this ServiceProvider provider)
            => provider.GetService<ChainService>();

        public static ShipService Ships(this ServiceProvider provider)
            => provider.GetService<ShipService>();

        public static PlayerService Players(this ServiceProvider provider)
            => provider.GetService<PlayerService>();
    }
}
