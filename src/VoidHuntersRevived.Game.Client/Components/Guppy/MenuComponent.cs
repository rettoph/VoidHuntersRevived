using Guppy;
using Guppy.Attributes;
using Guppy.Game.MonoGame;
using Guppy.Game.MonoGame.Providers;
using VoidHuntersRevived.Common.Messages;
using VoidHuntersRevived.Game.Core;

namespace VoidHuntersRevived.Game.Client.Components.Guppy
{
    //[AutoLoad]
    [GuppyFilter<MainMenuGuppy>()]
    internal sealed class MenuComponent : IGuppyComponent
    {
        private IMenuProvider _menus;

        public MenuComponent(IMenuProvider menus)
        {
            _menus = menus;
        }

        public void Initialize(IGuppy guppy)
        {
            _menus.Get(Menus.Main).Add(new[]
            {
                new MenuItem()
                {
                    Label = "Multiplayer",
                    OnClick = Launch<LocalGameGuppy>.Instance
                },
                new MenuItem()
                {
                    Label = "Singleplayer",
                    OnClick = Launch<LocalGameGuppy>.Instance
                }
            });
        }
    }
}
