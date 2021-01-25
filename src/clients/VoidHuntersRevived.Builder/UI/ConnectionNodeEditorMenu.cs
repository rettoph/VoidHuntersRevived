using Guppy.DependencyInjection;
using Guppy.UI.Elements;
using Guppy.UI.Enums;
using Guppy.UI.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Builder.UI
{
    public class ConnectionNodeEditorMenu : SecretContainer<IElement>
    {
        #region Private Fields
        private SingleInput _x;
        private SingleInput _y;
        private RadianInput _rotation;
        #endregion

        #region Public Properties
        public Vector2 Position
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
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.inline = InlineType.Both;

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
                input.Bounds.Width = 1f;
                input.Bounds.Y = 65;
            });
        }
        #endregion
    }
}
