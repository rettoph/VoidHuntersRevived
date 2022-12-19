using Guppy.Attributes;
using Guppy.Loaders;
using Guppy.Network.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Components;
using VoidHuntersRevived.Library.Mappers;
using VoidHuntersRevived.Library.Systems;

namespace VoidHuntersRevived.Library.Loaders
{
    [AutoLoad]
    internal sealed class EntityServiceLoader : IServiceLoader
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSystem<TickRemoteMasterSystem>()
                    .AddSystem<TickRemoteSlaveSystem>()
                    .AddSystem<PilotSystem>()
                    .AddSystem<UserPilotSystem>()
                    .AddSystem<UserPilotRemoteMasterSystem>()
                    .AddSystem<CurrentUserRemoteSlaveSystem>()
                    .AddSystem<UserRemoteMasterSystem>()
                    .AddSystem<PilotableAetherSystem>()
                    .AddSystem<GameStateRemoteSlaveSystem>()
                    .AddSystem<AetherDebugRemoteMasterSystem>();

            services.AddComponentType<Piloting>()
                    .AddComponentType<User>()
                    .AddComponentType<Rootable>()
                    .AddComponentType<Body>()
                    .AddComponentType<Pilotable>();

            services.AddScoped<PilotIdMap>();
        }
    }
}
