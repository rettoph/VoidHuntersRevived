using Guppy;
using Guppy.Attributes;
using Guppy.Loaders;
using Guppy.MonoGame.Providers;
using Guppy.MonoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Messages;
using VoidHuntersRevived.Common.Editor;
using Guppy.Common;
using Guppy.MonoGame.Messages;

namespace VoidHuntersRevived.Domain.Editor.Loaders
{
    [AutoLoad]
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
                    Label = "ShipPart Editor",
                    OnClick = Launch<EditorGuppy>.Instance
                }
            });
        }
    }
}
