using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Files.Common.Enums;
using Guppy.Core.Files.Common.Helpers;
using Guppy.Core.Files.Common.Services;
using Guppy.Core.Resources.Serialization.Json;
using Guppy.Engine.Common.Loaders;
using Guppy.Engine.Extensions.Autofac;
using Serilog;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
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
                config.Destructure.AsScalar<VhId>();

                if (scope.IsRoot())
                {
                    var fileTypePaths = scope.Resolve<IPathService>();
                    var source = fileTypePaths.GetSourceLocation(DirectoryType.AppData, "logs", $"log_{DateTime.Now.ToString("yyyy-dd-M")}.txt");
                    DirectoryHelper.EnsureDirectoryExists(source);

                    config
                        .WriteTo.File(
                            path: source.Path,
                            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                            retainedFileCountLimit: 5,
                            shared: true
                        )
                        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
                }
            });

            services.RegisterType<Fix64Converter>().As<JsonConverter>().SingleInstance();
            services.RegisterType<FixPolarConverter>().As<JsonConverter>().SingleInstance();
            services.RegisterType<FixVector2Converter>().As<JsonConverter>().SingleInstance();
            services.RegisterType<NativeDynamicArrayCastJsonConverter>().As<JsonConverter>().SingleInstance();

            services.RegisterInstance<PolymorphicJsonType>(new PolymorphicJsonType<Fix64, object>(nameof(Fix64))).SingleInstance();
        }
    }
}
