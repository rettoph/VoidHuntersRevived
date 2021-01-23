using Guppy.DependencyInjection;
using Guppy.UI.Elements;
using Guppy.UI.Enums;
using Guppy.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Builder.UI
{
    public class SimpleInput : SecretContainer<IElement>
    {
        #region Private Fields
        protected TextInput input { get; private set; }
        protected TextElement label { get; private set; }
        #endregion

        #region Public Properties
        public String Label 
        {
            set => this.label.Value = value;
        }

        public virtual String Value
        {
            get => this.input.Value;
            set => this.input.Value = value;
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.inline = InlineType.Vertical;

            this.label = this.inner.Children.Create<TextElement>((label, p, c) =>
            {
                label.Color[ElementState.Default] = p.GetColor("ui:color:1");
                label.Value = "Label";
            });

            this.input = this.inner.Children.Create<TextInput>((input, p, c) =>
            {
                input.Color[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderColor[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderWidth[ElementState.Default] = 1;
                input.Inline = InlineType.None;

                input.Bounds.Width = 1f;
                input.Bounds.Height = 35;
                input.Bounds.Y = 25;
                input.Padding.Left = 7;
                input.Padding.Right = 7;
            });
        }
        #endregion
    }
}
