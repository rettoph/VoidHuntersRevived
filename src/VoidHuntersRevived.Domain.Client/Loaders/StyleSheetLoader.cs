using Guppy.Attributes;
using Guppy.GUI;
using Guppy.GUI.Elements;
using Guppy.GUI.Loaders;
using Guppy.GUI.Providers;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Constants;
using VoidHuntersRevived.Domain.Client.Constants;

namespace VoidHuntersRevived.Domain.Client.Loaders
{
    [AutoLoad]
    internal sealed class StyleSheetLoader : IStyleSheetLoader
    {
        public void Configure(IStyleSheet styleSheet)
        {
            // throw new NotImplementedException();
        }

        public void Load(IStyleSheetProvider styles)
        {
            styles.Get(StyleSheets.Main)
                .Set<Unit>(Property.Width, Selector.Create<Stage>(), 1f)
                .Set(Property.Padding, Selector.Create<Label>("test"), new Padding(15, 15, 15, 15))
                .Configure(Selector.Create<Element>(), manager =>
                {
                    manager.Set(Property.Padding, new Padding(5, 5, 5, 5))
                        .Set(Property.Alignment, Alignment.TopLeft);
                })
                .Configure(Selector.Create<Label>(), manager =>
                {
                    manager.Set(Property.Inline, true)
                        .Set(Property.Font, Fonts.Default)
                        .Set(Property.Color, Color.Red)
                        .Set(Property.Color, ElementState.Hovered, Color.Green)
                        .Set(Property.BackgroundColor, ElementState.Hovered, Color.LightBlue);
                })
                .Configure(Selector.Create<ScrollContainer<Element>>(), manager =>
                {
                    manager.Set(Property.ScrollThumbColor, Color.Green)
                        .Set(Property.ScrollTrackColor, Color.Yellow)
                        .Set(Property.ScrollTrackWidth, 15);
                })
                .Configure(Selector.Create<TextInput>(), manager =>
                {
                    manager.Set(Property.Width, 100)
                        .Set(Property.Height, 100)
                        .Set(Property.BackgroundColor, Color.Pink)
                        .Set(Property.BackgroundColor, ElementState.Hovered, Color.Yellow)
                        .Set(Property.BackgroundColor, ElementState.Focused, Color.Red)
                        .Set(Property.BackgroundColor, ElementState.Active, Color.Green);
                });
        }
    }
}
