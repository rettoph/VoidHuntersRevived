using Guppy;
using Guppy.Attributes;
using Guppy.Loaders;
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
    [GuppyFilter<MainMenuGuppy>()]
    internal sealed class MenuLoader : IGuppyLoader
    {
        private IMenuProvider _menus;

        public MenuLoader(IMenuProvider menus)
        {
            _menus = menus;
        }

        public void Load(IGuppy guppy)
        {
            _menus.Get(Menus.Main).Add(new[]
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
