using Autofac;
using Guppy.Attributes;
using Guppy.Common.Autofac;
using Guppy.Common.Extensions.Autofac;
using Guppy.Extensions.Autofac;
using Guppy.Files.Enums;
using Guppy.Files.Helpers;
using Guppy.Files.Providers;
using Guppy.Loaders;
using Guppy.MonoGame;
using Guppy.MonoGame.Extensions.Serilog;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Serilog;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Domain.GameComponents;
using VoidHuntersRevived.Domain.Serialization.Json;

namespace VoidHuntersRevived.Domain.Loaders
{
    [AutoLoad]
    public sealed class DomainLoader : IServiceLoader
    {
        public void ConfigureServices(ContainerBuilder services)
        {
            services.Configure<LoggerConfiguration>((scope, config) =>
            {
                var fileTypePaths = scope.Resolve<IFileTypePathProvider>();
                var path = fileTypePaths.GetFullPath(FileType.AppData, Path.Combine("logs", $"log_{DateTime.Now.ToString("yyyy-dd-M")}.txt"));
                DirectoryHelper.EnsureDirectoryExists(path);

                config.Destructure.AsScalar<VhId>();

                config
                    .WriteTo.File(
                        path: path,
                        outputTemplate: "[{PeerType}][{SimulationType}][{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                        retainedFileCountLimit: 5,
                        shared: true
                    );

                try
                {
                    var terminal = scope.Resolve<ITerminal>();
                    config.WriteTo.Terminal(terminal, "[{PeerType}][{SimulationType}][{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
                }
                catch
                {
                    config.WriteTo.Console(outputTemplate: "[{PeerType}][{SimulationType}][{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
                }
            });

            services.RegisterType<Fix64Converter>().As<JsonConverter>().SingleInstance();
            services.RegisterType<FixPolarConverter>().As<JsonConverter>().SingleInstance();
            services.RegisterType<FixVector2Converter>().As<JsonConverter>().SingleInstance();
            services.RegisterType<NativeDynamicArrayCastJsonConverter>().As<JsonConverter>().SingleInstance();
        }
    }
}
