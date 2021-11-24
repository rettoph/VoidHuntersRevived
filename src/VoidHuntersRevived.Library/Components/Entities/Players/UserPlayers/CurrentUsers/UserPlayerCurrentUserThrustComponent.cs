using Guppy.DependencyInjection;
using Guppy.Network.Enums;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;
using VoidHuntersRevived.Library.Components.Entities.Ships;
using VoidHuntersRevived.Library.Enums;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Structs;
using VoidHuntersRevived.Library.Globals.Constants;

namespace VoidHuntersRevived.Library.Components.Entities.Players
{
    internal sealed class UserPlayerCurrentUserThrustComponent : UserPlayerCurrentUserCommandActionBaseComponent<DirectionState>
    {
        public override UInt32 ActionRequestMessageType => Messages.UserPlayer.RequestDirectionChanged;

        public override String CurrentUserCommandInput => "ship thrust";

        public override ICommandHandler CurrentUserCommandHandler => CommandHandler.Create<Direction, Boolean, IConsole>(this.HandleSetDirectionCommand);

        protected override Boolean TryDoActionRequest(DirectionState request, out DirectionState response)
        {
            if (this.Entity.Ship?.Components.Get<ShipThrustersComponent>().TrySetDirection(request) ?? false)
            {
                response = request;
                return true;
            }
            else
            {
                response = default;
                return false;
            }
        }

        protected override DirectionState ReadCurrentUserActionRequestMessage(NetIncomingMessage im)
        {
            return new DirectionState(
                direction: im.ReadEnum<Direction>(),
                state: im.ReadBoolean());
        }

        protected override void WriteCurrentUserActionRequestMessage(NetOutgoingMessage om, DirectionState request)
        {
            om.Write<Direction>(request.Direction);
            om.Write(request.State);
        }

        private void HandleSetDirectionCommand(Direction direction, bool state, IConsole arg3)
        {
            this.TryDoActionRequest(new DirectionState(
                direction: direction,
                state: state));
        }
    }
}
