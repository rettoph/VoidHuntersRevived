using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces.Factories;
using VoidHuntersRevived.Game.Pieces.Factories;
using VoidHuntersRevived.Game.Pieces.Services;

namespace VoidHuntersRevived.Game.Pieces.Loaders
{
    [AutoLoad]
    internal sealed class PieceLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterType<TreeFactory>().AsImplementedInterfaces().InstancePerLifetimeScope();
            services.RegisterType<NodeFactory>().AsImplementedInterfaces().InstancePerLifetimeScope();

            services.RegisterType<SocketService>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
