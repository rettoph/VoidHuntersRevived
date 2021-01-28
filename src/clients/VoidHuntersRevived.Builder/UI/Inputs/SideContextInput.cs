using Guppy.DependencyInjection;
using Guppy.UI.Elements;
using Guppy.UI.Enums;
using Guppy.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Extensions.DependencyInjection;
using VoidHuntersRevived.Builder.Contexts;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;
using Guppy.Events.Delegates;

namespace VoidHuntersRevived.Builder.UI.Inputs
{
    public class SideContextInput : SecretContainer<IElement>
    {
        #region Private Fields
        private TextInput _rotation;
        private TextInput _length;
        #endregion

        #region Public Fields
        public SideContext Value
        {
            get => new SideContext()
            {
                Length = this.GetLength(),
                Rotation = this.GetRotation()
            };
            set
            {
                _rotation.Value = MathHelper.ToDegrees(value.Rotation).ToString("##0.####");
                _length.Value = value.Length.ToString("##0.####");
            }
        }
        #endregion

        #region Events
        public event OnEventDelegate<TextElement, String> OnValueChanged;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.inline = InlineType.Both;

            _rotation = this.inner.Children.Create<TextInput>((input, p, c) =>
            {
                input.Color[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderColor[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderWidth[ElementState.Default] = 1;
                input.Inline = InlineType.None;
                input.Filter = new Regex("^[0-9\\.]{0,10}$");

                input.Bounds.Width = 0.5f;
                input.Bounds.Height = 35;
                input.Padding.Left = 7;
                input.Padding.Right = 7;

                input.OnValueChanged += this.HandleValueChanged;
            });

            _length = this.inner.Children.Create<TextInput>((input, p, c) =>
            {
                input.Color[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderColor[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderWidth[ElementState.Default] = 1;
                input.Inline = InlineType.None;
                input.Filter = new Regex("^[0-9\\.]{0,10}$");

                input.Bounds.Width = 0.5f;
                input.Bounds.Height = 35;
                input.Bounds.X = 0.5f;
                input.Padding.Left = 7;
                input.Padding.Right = 7;

                input.OnValueChanged += this.HandleValueChanged;
            });
        }

        protected override void Release()
        {
            base.Release();

            _rotation.OnValueChanged -= this.HandleValueChanged;
            _length.OnValueChanged -= this.HandleValueChanged;

            _rotation = null;
            _length = null;
        }
        #endregion

        #region Helper Methods
        private Single GetRotation()
        {
            Single output;
            if (Single.TryParse(_rotation.Value, out output))
                return MathHelper.ToRadians(output);

            return 0;
        }

        private Single GetLength()
        {
            Single output;
            if (Single.TryParse(_length.Value, out output))
                return output;

            return 1;
        }
        #endregion

        #region Event Handlers
        private void HandleValueChanged(TextElement sender, string args)
            => this.OnValueChanged?.Invoke(sender, args);
        #endregion
    }
}
