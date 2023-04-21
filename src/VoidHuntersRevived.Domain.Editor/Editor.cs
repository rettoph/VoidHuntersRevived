using Guppy.GUI.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Editor;
using VoidHuntersRevived.Common.Editor.Constants;
using VoidHuntersRevived.Common.Entities.ShipParts;

namespace VoidHuntersRevived.Domain.Editor
{
    internal sealed class Editor : IEditor
    {
        public ScrollBox<Element> ControlPanel { get; } = new ScrollBox<Element>(ElementNames.EditorControlPanel.Yield());

        public ShipPartResource ShipPartResource { get; } = new ShipPartResource(nameof(Editor));
    }
}
