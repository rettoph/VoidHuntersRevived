using Guppy.CommandLine.Interfaces;
using Guppy.CommandLine.Services;
using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Threading.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Library.Components.Players.UserPlayers
{
    public abstract class CurrentUserPlayerBaseComponent : Component<UserPlayer>
    {
        protected CommandService commands { get; private set; }

        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.commands = provider.GetCommands();
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            if(this.Entity.IsCurrentUser)
            {
                this.InitializeCurrentUser(provider);
            }
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            if (this.Entity.IsCurrentUser)
            {
                this.UninitializeCurrentUser();
            }
        }

        protected abstract void InitializeCurrentUser(ServiceProvider provider);
        protected abstract void UninitializeCurrentUser();
    }
}
