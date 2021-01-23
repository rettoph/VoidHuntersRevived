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

namespace VoidHuntersRevived.Builder.UI
{
    public class SideContextInput : SecretContainer<IElement>
    {
        #region Private Fields
        private TextInput _rotation;
        private TextInput _length;
        #endregion

        #region Internal Fields
        internal SideContext source;
        #endregion

        #region Public Properties
        public SideContext SideContext => new SideContext()
        {
            Rotation = this.GetRotation(),
            Length = this.GetLength()
        };
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.inline = InlineType.Both;

            _rotation = this.inner.Children.Create<TextInput>((input, p, c) =>
            {
                input.Value = MathHelper.ToDegrees(this.source.Rotation).ToString("##0.####");
                input.Color[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderColor[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderWidth[ElementState.Default] = 1;
                input.Inline = InlineType.None;
                input.Filter = new Regex("^[0-9\\.]{0,10}$");

                input.Bounds.Width = 0.5f;
                input.Bounds.Height = 35;
                input.Padding.Left = 7;
                input.Padding.Right = 7;
            });

            _length = this.inner.Children.Create<TextInput>((input, p, c) =>
            {
                input.Value = this.source.Length.ToString("##0.####");
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
            });
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
    }
}
