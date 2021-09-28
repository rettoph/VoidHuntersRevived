﻿using Guppy.DependencyInjection;
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

namespace VoidHuntersRevived.Library.Components.Entities.Players
{
    internal sealed class UserPlayerCurrentUserDirectionComponent : UserPlayerCurrentUserCommandActionBaseComponent<DirectionState>
    {
        public override UInt32 ActionRequestMessageType => Constants.Messages.UserPlayer.RequestDirectionChanged;

        public override String CurrentUserCommandInput => "ship set direction";

        public override ICommandHandler CurrentUserCommandHandler => CommandHandler.Create<Direction, Boolean, IConsole>(this.HandleSetDirectionCommand);

        protected override Boolean TryDoActionRequest(DirectionState request, out DirectionState response)
        {
            if (this.Entity.Ship?.Components.Get<ShipDirectionComponent>().TrySetDirection(request) ?? false)
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
