using Guppy.Attributes;
using Guppy.GUI;
using Guppy.GUI.Elements;
using Guppy.GUI.Loaders;
using Guppy.GUI.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Constants;

namespace VoidHuntersRevived.Domain.Client.Loaders
{
    [AutoLoad]
    internal sealed class StyleSheetLoader : IStyleSheetLoader
    {
        public void Load(IStyleSheetProvider styles)
        {
            styles.Get(StyleSheets.Main).Set<Unit>(Property.Width, Selector.Create<Stage>(), 1f);
            styles.Get(StyleSheets.Main).Set<Unit>(Property.Width, Selector.Create<Element>(), 0.25f);
            styles.Get(StyleSheets.Main).Set(Property.Padding, Selector.Create<Element>(), new Padding(5, 5, 5, 5));
        }
    }
}
