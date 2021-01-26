using Guppy.DependencyInjection;
using Guppy.UI.Elements;
using Guppy.UI.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Builder.UI.Inputs
{
    public class ColorInput : BaseLabeledInput<Color>
    {
        #region Private Fields
        protected TextInput _r;
        protected TextInput _g;
        protected TextInput _b;
        protected TextInput _a;
        #endregion

        #region Public Properties
        public override Color Value
        {
            get => new Color(this.GetByteValue(_r), this.GetByteValue(_g), this.GetByteValue(_b), this.GetByteValue(_a));
            set
            {
                _r.Value = value.R.ToString("##0");
                _g.Value = value.G.ToString("##0");
                _b.Value = value.B.ToString("##0");
                _a.Value = value.A.ToString("##0");
            }
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _r = this.inner.Children.Create<TextInput>((input, p, c) =>
            {
                input.Color[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderColor[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderWidth[ElementState.Default] = 1;
                input.Inline = InlineType.None;

                input.Bounds.X = 0f;
                input.Bounds.Width = 0.25f;
                input.Bounds.Height = 35;
                input.Bounds.Y = 25;
                input.Padding.Left = 7;
                input.Padding.Right = 7;
            });

            _g = this.inner.Children.Create<TextInput>((input, p, c) =>
            {
                input.Color[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderColor[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderWidth[ElementState.Default] = 1;
                input.Inline = InlineType.None;

                input.Bounds.X = 0.25f;
                input.Bounds.Width = 0.25f;
                input.Bounds.Height = 35;
                input.Bounds.Y = 25;
                input.Padding.Left = 7;
                input.Padding.Right = 7;
            });

            _b = this.inner.Children.Create<TextInput>((input, p, c) =>
            {
                input.Color[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderColor[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderWidth[ElementState.Default] = 1;
                input.Inline = InlineType.None;

                input.Bounds.X = 0.5f;
                input.Bounds.Width = 0.25f;
                input.Bounds.Height = 35;
                input.Bounds.Y = 25;
                input.Padding.Left = 7;
                input.Padding.Right = 7;
            });

            _a = this.inner.Children.Create<TextInput>((input, p, c) =>
            {
                input.Color[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderColor[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderWidth[ElementState.Default] = 1;
                input.Inline = InlineType.None;

                input.Bounds.X = 0.75f;
                input.Bounds.Width = 0.25f;
                input.Bounds.Height = 35;
                input.Bounds.Y = 25;
                input.Padding.Left = 7;
                input.Padding.Right = 7;
            });
        }
        #endregion

        #region Helper Methods
        private Byte GetByteValue(TextInput input)
        {
            Byte value;

            if (Byte.TryParse(input.Value, out value))
                return value;

            return 0;
        }
        #endregion
    }
}
