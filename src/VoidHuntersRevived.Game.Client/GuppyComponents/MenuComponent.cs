using Guppy;
using Guppy.Attributes;
using Guppy.Loaders;
using Guppy.MonoGame;
using Guppy.MonoGame.Loaders;
using Guppy.MonoGame.Providers;
using VoidHuntersRevived.Common.Messages;
using VoidHuntersRevived.Game.Common;

namespace VoidHuntersRevived.Game.Client.GuppyComponents
{
    [AutoLoad]
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
