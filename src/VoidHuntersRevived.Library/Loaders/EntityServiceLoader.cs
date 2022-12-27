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
using VoidHuntersRevived.Library.Simulations.Systems.Lockstep;

namespace VoidHuntersRevived.Library.Loaders
{
    [AutoLoad(0)]
    internal sealed class EntityServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddService<LockstepTickRemoteMasterSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddService<LockstepTickRemoteSlaveSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddService<LockstepPilotSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddService<LockstepUserPilotSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddService<LockstepUserPilotRemoteMasterSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddService<LockstepCurrentUserRemoteSlaveSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddService<LockstepUserRemoteMasterSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddService<LockstepPilotableAetherSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddService<LockstepAetherSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddService<LockstepAetherDebugRemoteMasterSystem>()
                .SetLifetime(ServiceLifetime.Scoped)
                .AddInterfaceAliases();

            services.AddScoped<PilotIdMap>();
        }
    }
}
