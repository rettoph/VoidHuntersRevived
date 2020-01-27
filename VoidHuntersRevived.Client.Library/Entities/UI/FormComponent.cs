using Guppy.Loaders;
using Guppy.UI.Entities.UI;
using Guppy.UI.Utilities.Units;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Entities.UI
{
    public class FormComponent : StyleElement
    {
        private TextElement _label;
        private TextInput _input;
        private ContentLoader _content;

        public String Label { get => _label.Text; set => _label.Text = value; }
        public String Value { get => _input.Text; set => _input.Text = value; }

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _content = provider.GetRequiredService<ContentLoader>();
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            this.Bounds.Set(0, 0, 1f, 70);
            this.BorderSize = 0;

            _label = this.add<TextElement>(t =>
            {
                t.Bounds.Set(15, 15, new Unit[] { 1f, -30 }, 20);
                t.TextAlignment = BaseElement.Alignment.CenterLeft;
                t.TextColor = new Color(222, 229, 229);
                t.Font = _content.TryGet<SpriteFont>("font:ui:label");
            });

            _input = this.add<TextInput>(i =>
            {
                i.Bounds.Set(15, 40, new Unit[] { 1f, -30 }, 30);
                i.BorderColor = new Color(210, 216, 216);
                i.BackgroundColor = new Color(222, 229, 229);
                i.TextColor = Color.Black;
                i.Font = _content.TryGet<SpriteFont>("font:ui:input");
            });
        }
        #endregion
    }
}
