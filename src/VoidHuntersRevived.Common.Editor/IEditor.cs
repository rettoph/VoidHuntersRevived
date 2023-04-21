using Guppy.GUI.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.ShipParts;

namespace VoidHuntersRevived.Common.Editor
{
    public interface IEditor
    {
        ScrollBox<Element> ControlPanel { get; }
        ShipPartResource ShipPartResource { get; }
    }
}
