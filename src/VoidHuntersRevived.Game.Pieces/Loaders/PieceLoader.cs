﻿using Autofac;
using Guppy.Attributes;
using Guppy.Loaders;
using Guppy.Network;
using Guppy.Resources.Serialization.Json.Converters;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Game.Pieces.Services;

namespace VoidHuntersRevived.Game.Pieces.Loaders
{
    [AutoLoad]
    internal sealed class PieceLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.RegisterType<PolymorphicConverter<IPieceComponent>>().As<JsonConverter>().SingleInstance();

            services.RegisterType<TreeService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            services.RegisterType<NodeService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            services.RegisterType<SocketService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            services.RegisterType<VisibleRenderingService>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
