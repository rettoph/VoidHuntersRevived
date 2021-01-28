using Guppy.DependencyInjection;
using Guppy.UI.Elements;
using Guppy.UI.Enums;
using Guppy.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Builder.UI.Inputs
{
    public class ShapeTransformationsInput : SecretContainer<IElement>
    {
        #region Private Fields
        private SingleInput _x;
        private SingleInput _y;
        private RadianInput _rotation;
        private SingleInput _scale;
        #endregion

        #region Public Properties
        public Vector2 Translation
        {
            get => new Vector2(_x.Value, _y.Value);
            set
            {
                _x.Value = value.X;
                _y.Value = value.Y;
            }
        }
        public Single Rotation
        {
            get => _rotation.Value;
            set => _rotation.Value = value;
        }
        public Single Scale
        {
            get => Math.Max(_scale.Value, 0.01f);
            set => _scale.Value = value;
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.inline = InlineType.Both;
            this.Padding.Bottom = 15;
            this.Padding.Top = 15;

            _x = this.inner.Children.Create<SingleInput>((input, p, c) =>
            {
                input.Label = "x";
                input.Bounds.Width = 0.5f;
            });

            _y = this.inner.Children.Create<SingleInput>((input, p, c) =>
            {
                input.Label = "y";
                input.Bounds.Width = 0.5f;
                input.Bounds.X = 0.5f;
            });

            _rotation = this.inner.Children.Create<RadianInput>((input, p, c) =>
            {
                input.Label = "Rotation";
                input.Bounds.Width = 0.5f;
                input.Bounds.Y = 65;
            });

            _scale = this.inner.Children.Create<SingleInput>((input, p, c) =>
            {
                input.Label = "Scale";
                input.Bounds.Width = 0.5f;
                input.Bounds.X = 0.5f;
                input.Bounds.Y = 65;
            });
        }

        protected override void Release()
        {
            base.Release();

            _x = null;
            _y = null;
            _rotation = null;
            _scale = null;
        }
        #endregion
    }
}
