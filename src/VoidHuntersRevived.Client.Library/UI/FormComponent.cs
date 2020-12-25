using Guppy.DependencyInjection;
using Guppy.Extensions.DependencyInjection;
using Guppy.UI.Elements;
using Guppy.UI.Enums;
using Guppy.UI.Interfaces;
using Guppy.UI.Utilities.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.UI
{
    /// <summary>
    /// Simple component that contains a text input & its lael
    /// </summary>
    public class FormComponent : SecretContainer<TextElement, StackContainer<TextElement>>
    {
        #region Public Properties
        public TextElement Label { get; private set; }
        public TextInput Input { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.inline = InlineType.Vertical;
            this.Bounds.X = new CustomUnit(c => (c - this.Bounds.Width.ToPixel(c)) / 2);
            this.Padding.Left = 10;
            this.Padding.Right = 10;
            this.Padding.Bottom = 10;
            this.Padding.Top = 10;

            this.OnContainerChanged += this.HandleContainerChanged;
        }

        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.inner.Alignment = StackAlignment.Vertical;
        }
        #endregion

        #region Event Handlers
        private void HandleContainerChanged(Element sender, IContainer old, IContainer value)
        {
            if(value == default)
            {
                this.Label?.TryRelease();
                this.Label = default;

                this.Input?.TryRelease();
                this.Input = default;
            }
            else
            {
                this.Label = this.inner.Children.Create<TextElement>((label, p, c) =>
                {
                    label.Color[ElementState.Default] = p.GetColor("ui:input:color:1");
                    label.Inline = InlineType.None;
                    label.Font = p.GetContent<SpriteFont>("font:ui:normal");
                    label.Bounds.Height = 30;
                    label.Bounds.Width = 1f;
                    label.Alignment = Alignment.TopLeft;
                });
                this.Input = this.inner.Children.Create<TextInput>((input, p, c) =>
                {
                    input.Color[ElementState.Default] = p.GetColor("ui:input:color:1");
                    input.BorderWidth[ElementState.Default] = 1;
                    input.BorderColor[ElementState.Default] = p.GetColor("ui:input:color:2");
                    input.BackgroundColor[ElementState.Default] = new Color(Color.Black, 100);
                    input.Font = p.GetContent<SpriteFont>("font:ui:light");
                    input.Bounds.Height = 35;
                    input.Bounds.Width = 1f;
                    input.Padding.Top = 5;
                    input.Padding.Right = 7;
                    input.Padding.Bottom = 5;
                    input.Padding.Left = 7;
                });
            }
        }
        #endregion
    }
}
