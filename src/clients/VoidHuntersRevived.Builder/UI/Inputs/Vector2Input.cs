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
    public class Vector2Input : BaseLabeledInput<Vector2>
    {
        #region Private Fields
        protected TextInput _x;
        protected TextInput _y;
        #endregion

        #region Public Properties
        public override Vector2 Value
        {
            get => new Vector2(this.GetValue(_x), this.GetValue(_y));
            set
            {
                _x.Value = value.X.ToString("#,##0.#####");
                _y.Value = value.Y.ToString("#,##0.#####");
            }
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            _x = this.inner.Children.Create<TextInput>((input, p, c) =>
            {
                input.Color[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderColor[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderWidth[ElementState.Default] = 1;
                input.Inline = InlineType.None;

                input.Bounds.X = 0f;
                input.Bounds.Width = 0.5f;
                input.Bounds.Height = 35;
                input.Bounds.Y = 25;
                input.Padding.Left = 7;
                input.Padding.Right = 7;

                input.OnValueChanged += this.HandleValueChanged;
            });

            _y = this.inner.Children.Create<TextInput>((input, p, c) =>
            {
                input.Color[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderColor[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderWidth[ElementState.Default] = 1;
                input.Inline = InlineType.None;

                input.Bounds.X = 0.5f;
                input.Bounds.Width = 0.5f;
                input.Bounds.Height = 35;
                input.Bounds.Y = 25;
                input.Padding.Left = 7;
                input.Padding.Right = 7;

                input.OnValueChanged += this.HandleValueChanged;
            });
        }

        protected override void Release()
        {
            base.Release();

            _x.OnValueChanged -= this.HandleValueChanged;
            _y.OnValueChanged -= this.HandleValueChanged;

            _x = null;
            _y = null;
        }
        #endregion

        #region Helper Methods
        private Single GetValue(TextInput input)
        {
            Single value;

            if (Single.TryParse(input.Value, out value))
                return value;

            return 0;
        }
        #endregion
    }
}
