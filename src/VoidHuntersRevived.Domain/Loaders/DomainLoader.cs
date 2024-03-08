using Autofac;
using Guppy.Attributes;
using Guppy.Common.Autofac;
using Guppy.Extensions.Autofac;
using Guppy.Files.Enums;
using Guppy.Files.Helpers;
using Guppy.Files.Providers;
using Guppy.Loaders;
using Guppy.Resources.Serialization.Json;
using Serilog;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Core;
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

                if (scope.IsTag(LifetimeScopeTags.MainScope))
                {
                    var fileTypePaths = scope.Resolve<IFilePathProvider>();
                    var path = fileTypePaths.GetFullPath(FileType.AppData, Path.Combine("logs", $"log_{DateTime.Now.ToString("yyyy-dd-M")}.txt"));
                    DirectoryHelper.EnsureDirectoryExists(path);

                    config
                        .WriteTo.File(
                            path: path,
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
