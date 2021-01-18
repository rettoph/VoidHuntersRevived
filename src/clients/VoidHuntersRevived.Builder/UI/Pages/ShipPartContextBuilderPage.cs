using Guppy.DependencyInjection;
using Guppy.UI.Elements;
using Guppy.UI.Enums;
using Guppy.UI.Interfaces;
using Guppy.UI.Utilities.Units;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Builder.UI.Pages
{
    /// <summary>
    /// The main page overlayed onto the game scene.
    /// This should be used when adding custom pages into
    /// a service.
    /// </summary>
    public class ShipPartContextBuilderPage : SecretContainer<IElement>, IPage
    {

        #region Public Properties
        public TextElement PrevButton { get; private set; }
        public TextElement NextButton { get; private set; }
        public PageContainer Pages { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.PrevButton = this.inner.Children.Create<TextElement>("ui:button:0", (button, p, c) =>
            {
                button.Padding.Left = 7;
                button.Padding.Right = 7;
                button.Bounds.Y = new CustomUnit(c => c - button.Bounds.Height.ToPixel(c) - 15);
                button.Bounds.X = 15;
                button.Inline = InlineType.Horizontal;
            });

            this.NextButton = this.inner.Children.Create<TextElement>("ui:button:0", (button, p, c) =>
            {
                button.Padding.Left = 7;
                button.Padding.Right = 7;
                button.Bounds.Y = new CustomUnit(c => c - button.Bounds.Height.ToPixel(c) - 15);
                button.Bounds.X = new CustomUnit(c => c - button.Bounds.Width.ToPixel(c) - 15);
                button.Inline = InlineType.Horizontal;
            });

            this.Pages = this.inner.Children.Create<PageContainer>();
        }
        #endregion
    }
}
