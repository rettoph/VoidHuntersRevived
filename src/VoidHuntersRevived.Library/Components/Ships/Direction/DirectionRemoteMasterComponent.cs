using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Messages.Network;
using VoidHuntersRevived.Library.Messages.Requests;

namespace VoidHuntersRevived.Library.Components.Ships
{
    [HostTypeRequired(HostType.Remote)]
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal sealed class DirectionRemoteMasterComponent : ReferenceComponent<Ship, DirectionComponent>
    {
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Reference.OnDirectionChanged += this.HandleDirectionChanged;
        }

        protected override void Uninitialize()
        {
            base.Uninitialize();

            this.Reference.OnDirectionChanged -= this.HandleDirectionChanged;
        }

        private void HandleDirectionChanged(DirectionComponent sender, DirectionRequest args)
        {
            this.Entity.SendMessage<ShipDirectionChangedMessage>(new ShipDirectionChangedMessage()
            {
                Direction = args.Direction,
                State = args.State
            });
        }
    }
}
