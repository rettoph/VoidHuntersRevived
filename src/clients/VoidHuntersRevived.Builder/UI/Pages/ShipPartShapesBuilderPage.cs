using Guppy.DependencyInjection;
using Guppy.UI.Elements;
using Guppy.UI.Enums;
using Guppy.UI.Interfaces;
using Guppy.UI.Utilities.Units;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Graphics;

namespace VoidHuntersRevived.Builder.UI.Pages
{
    public class ShipPartShapesBuilderPage : SecretContainer<IElement>, IPage
    {
        #region Private Fields
        private TextElement _message;
        private TextElement _subMessage;
        #endregion

        #region Public Properties
        public TextElement AddShapeButton { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Padding.Top = 15;
            this.Padding.Right = 15;
            this.Padding.Bottom = 15;
            this.Padding.Left = 15;

            _message = this.inner.Children.Create<TextElement>((message, p, c) =>
            {
                message.Bounds.X = new CustomUnit(c => (c - message.Bounds.Width.ToPixel(c)) / 2);
                message.Color[ElementState.Default] = Color.White;
                message.Inline = InlineType.Both;
            });

            _subMessage = this.inner.Children.Create<TextElement>((subMessage, p, c) =>
            {
                subMessage.Bounds.X = new CustomUnit(c => (c - subMessage.Bounds.Width.ToPixel(c)) / 2);
                subMessage.Bounds.Y = 20;
                subMessage.Color[ElementState.Default] = Color.White;
                subMessage.Font = p.GetContent<SpriteFont>("debug:font:small");
                subMessage.Inline = InlineType.Both;
            });

            this.AddShapeButton = this.inner.Children.Create<TextElement>("ui:button:0", (button, p, c) =>
            {
                button.Value = "Add Shape";
            });
        }
        #endregion

        #region API
        public void SetMessage(String message, Color? color = null)
        {
            _message.Value = message;
            _message.Color[ElementState.Default] = color ?? Color.White;
        }

        public void SetSubMessage(String message, Color? color = null)
        {
            _subMessage.Value = message;
            _subMessage.Color[ElementState.Default] = color ?? _message.Color[ElementState.Default];
        }
        #endregion
    }
}
