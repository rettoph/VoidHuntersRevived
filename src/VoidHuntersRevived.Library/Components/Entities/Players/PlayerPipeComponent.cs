using Guppy.DependencyInjection;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Scenes;

namespace VoidHuntersRevived.Library.Components.Entities.Players
{
    internal sealed class PlayerPipeComponent : StaticPipeComponent<Player>
    {
        protected override IPipe GetPipe(GuppyServiceProvider provider, PrimaryScene scene)
            => scene.Channel.Pipes.GetOrCreateById(PipeIds.PlayersPipeId);
    }
}
