using Guppy.Attributes;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.DependencyInjection.Builders;
using Guppy.ServiceLoaders;
using Guppy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Client.Library.Graphics.Effects;
using VoidHuntersRevived.Client.Library.Graphics.Services;
using VoidHuntersRevived.Client.Library.Graphics.Vertices;

namespace VoidHuntersRevived.Client.Library.ServiceLoaders
{
    [AutoLoad]
    internal sealed class TrailServiceLoader : IServiceLoader
    {
        public void RegisterServices(AssemblyHelper assemblyHelper, ServiceProviderBuilder services)
        {
            services.RegisterService<TrailService>()
                .SetLifetime(ServiceLifetime.Scoped)
                .RegisterTypeFactory(factory =>
                {
                    factory.SetDefaultConstructor<TrailService>();
                });

            services.RegisterService<PrimitiveBatch<VertexTrail, TrailEffect>>()
                .SetLifetime(ServiceLifetime.Scoped)
                .RegisterTypeFactory(factory =>
                {
                    factory.SetMethod(p =>
                    {
                        var graphics = p.GetService<GraphicsDevice>();

                        return new PrimitiveBatch<VertexTrail, TrailEffect>(
                            new TrailEffect(graphics),
                            graphics);
                    });
                });
        }
    }
}
