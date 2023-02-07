using Guppy.Attributes;
using Guppy.MonoGame;
using Guppy.MonoGame.Loaders;
using Guppy.MonoGame.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Messages;

namespace VoidHuntersRevived.Domain.Client.Loaders
{
    [AutoLoad]
    internal sealed class MenuLoader : IMenuLoader
    {
        public void Load(IMenuProvider menus)
        {
            menus.Get(Menus.Main).Add(new[]
            {
                new MenuItem()
                {
                    Label = "Multiplayer",
                    OnClick = Launch<ClientGameGuppy>.Instance
                },
                new MenuItem()
                {
                    Label = "Singleplayer",
                    OnClick = Launch<ClientGameGuppy>.Instance
                }
            });
        }
    }
}
