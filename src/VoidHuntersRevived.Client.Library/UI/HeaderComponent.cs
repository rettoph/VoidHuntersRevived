using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.UI.Elements;
using Guppy.UI.Enums;
using Guppy.UI.Utilities.Units;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Windows.Library.UI
{
    public sealed class HeaderComponent : StackContainer
    {
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Padding.Bottom = 0.1f;

            this.Bounds.X = new CustomUnit(c => (c - this.Bounds.Width.ToPixel(c)) / 2);
            this.Bounds.Y = 100;
            this.Alignment = StackAlignment.Horizontal;
            this.inline = InlineType.Both;
            this.Children.Create<Element>((logo, p, c) =>
            {
                logo.Bounds.Height = 75;
                logo.Bounds.Width = 75;
                logo.Bounds.Y = 5;
                logo.BackgroundImage[ElementState.Default] = p.GetContent<Texture2D>("sprite:ui:logo");
            });

            this.Children.Create<StackContainer>((header, p, c) =>
            {
                header.Inline = InlineType.Both;
                header.Alignment = StackAlignment.Vertical;
                header.Children.Create<StackContainer>((title, p, c) =>
                {
                    title.Inline = InlineType.Both;
                    title.Bounds.Width = 1f;
                    title.Bounds.Height = 100;
                    title.Alignment = StackAlignment.Horizontal;
                    title.Children.Create<TextElement>("ui:label:title", (text, p, c) =>
                    {
                        text.Value = "Void Hunters";
                        text.Font = p.GetContent<SpriteFont>("font:ui:label:bold");
                        text.Inline = InlineType.Both;
                    });
                    title.Children.Create<TextElement>("ui:label:title", (text, p, c) =>
                    {
                        text.Value = " Revived";
                        text.Font = p.GetContent<SpriteFont>("font:ui:label:light");
                        text.Color[ElementState.Default] = p.GetColor("ui:label:color:2");
                        text.Inline = InlineType.Both;
                    });
                });
                header.Children.Create<TextElement>("ui:label:title:small", (text, p, c) =>
                {
                    text.Value = "Alpha 0.2.28";
                    text.Inline = InlineType.Both;
                });
            });
        }
    }
}
