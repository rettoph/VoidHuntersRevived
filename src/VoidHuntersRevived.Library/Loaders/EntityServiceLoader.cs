using Guppy.Attributes;
using Guppy.Common;
using Guppy.Loaders;
using Guppy.Network.Identity;
using Microsoft.Extensions.DependencyInjection;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Components;
using VoidHuntersRevived.Library.Mappers;
using VoidHuntersRevived.Library.Subscribers;
using VoidHuntersRevived.Library.Systems;

namespace VoidHuntersRevived.Library.Loaders
{
    [AutoLoad(0)]
    internal sealed class EntityServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ISubscriber, WorldTickSubscriber>();

            services.AddService<TickRemoteMasterSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddService<TickRemoteSlaveSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddService<PilotSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddService<UserPilotSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddService<UserPilotRemoteMasterSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddService<CurrentUserRemoteSlaveSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddService<UserRemoteMasterSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddService<PilotableAetherSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddService<GameStateRemoteSlaveSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddService<AetherDebugRemoteMasterSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddScoped<PilotIdMap>();
        }
    }
}
