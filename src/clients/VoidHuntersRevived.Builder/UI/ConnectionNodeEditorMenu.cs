using Guppy.DependencyInjection;
using Guppy.Events.Delegates;
using Guppy.UI.Elements;
using Guppy.UI.Enums;
using Guppy.UI.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Builder.UI.Inputs;

namespace VoidHuntersRevived.Builder.UI
{
    public class ConnectionNodeEditorMenu : SecretContainer<IElement>
    {
        #region Private Fields
        private SingleInput _x;
        private SingleInput _y;
        private RadianInput _rotation;
        private TextElement _deleteButton;
        #endregion

        #region Internal Fields
        internal Boolean deleteable;
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

        #region Events
        public event OnEventDelegate<ConnectionNodeEditorMenu> OnDelete;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.inline = InlineType.Both;

            if (this.deleteable)
            {
                _deleteButton = this.inner.Children.Create<TextElement>("ui:button:0", (delete, p, c) =>
                {
                    delete.Value = "Delete Node";
                    delete.Bounds.Width = 1f;
                    delete.OnClicked += this.HandleDeleteClicked;
                });
            }

            var offset = (this.deleteable ? 50 : 0);
            _x = this.inner.Children.Create<SingleInput>((input, p, c) =>
            {
                input.Label = "x";
                input.Bounds.Width = 0.5f;
                input.Bounds.Y = offset;
            });

            _y = this.inner.Children.Create<SingleInput>((input, p, c) =>
            {
                input.Label = "y";
                input.Bounds.Width = 0.5f;
                input.Bounds.X = 0.5f;
                input.Bounds.Y = offset;
            });

            _rotation = this.inner.Children.Create<RadianInput>((input, p, c) =>
            {
                input.Label = "Rotation";
                input.Bounds.Width = 1f;
                input.Bounds.Y = offset + 65;
            });
        }

        protected override void PreRelease()
        {
            base.PreRelease();

            if(_deleteButton != default)
            {
                _deleteButton.OnClicked -= this.HandleDeleteClicked;
            }
        }

        protected override void Release()
        {
            base.Release();

            _deleteButton = null;
            _x = null;
            _y = null;
            _rotation = null;
        }
        #endregion

        #region Event Handlers
        private void HandleDeleteClicked(Element sender)
            => this.OnDelete?.Invoke(this);
        #endregion
    }
}
