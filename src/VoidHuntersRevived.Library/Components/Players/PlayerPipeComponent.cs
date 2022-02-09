using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.Components.Players
{
    internal sealed class PlayerPipeComponent : StaticPipeComponent<Player>
    {
        protected override Pipe GetPipe(ServiceProvider provider, PrimaryScene scene)
            => scene.Room.Pipes.GetById(PipeIds.PlayersPipeId);
    }
}
