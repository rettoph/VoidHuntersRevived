using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;

namespace VoidHuntersRevived.Game.Ships.Loaders
{
    [AutoLoad]
    public class ShipLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            //
        }
    }
}
