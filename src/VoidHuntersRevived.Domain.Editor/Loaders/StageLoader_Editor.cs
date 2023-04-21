using Guppy.Attributes;
using Guppy.Common.Utilities;
using Guppy.GUI;
using Guppy.GUI.Elements;
using Guppy.GUI.Loaders;
using Guppy.MonoGame.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Common.Editor;
using VoidHuntersRevived.Common.Editor.Constants;

namespace VoidHuntersRevived.Domain.Editor.Loaders
{
    [AutoLoad]
    internal sealed class StageLoader_Editor : IStageLoader
    {
        public BlockList StageBlockList => BlockList.CreateWhitelist(Stages.Editor);

        private readonly IMenuProvider _menus;
        private readonly IEditor _editor;

        public StageLoader_Editor(IMenuProvider menus, IEditor editor)
        {
            _menus = menus;
            _editor = editor;
        }

        public void Load(Stage stage)
        {
            var menu = new TextButtonMenu(_menus.Get(Menus.Editor), ElementNames.EditorMenu);

            stage.Add(menu);
            stage.Add(_editor.ControlPanel);
        }
    }
}
