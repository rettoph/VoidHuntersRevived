using Guppy;
using Guppy.Attributes;
using Guppy.Loaders;
using Guppy.MonoGame;
using Guppy.MonoGame.Loaders;
using Guppy.MonoGame.Providers;
using VoidHuntersRevived.Common.Messages;

namespace VoidHuntersRevived.Game.Client.Loaders
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
