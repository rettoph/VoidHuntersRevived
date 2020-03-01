using Guppy.Loaders;
using Guppy.UI.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Entities.UI
{
    public class FormComponent : ProtectedContainer<Element>
    {
        #region Private Fields
        private TextElement _label;
        private TextInput _input;
        private ContentLoader _content;
        #endregion

        #region Public Fields
        public String Label { get => _label.Text; set => _label.Text = value; }
        public String Value { get => _input.Value; set => _input.Value = value; }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _content = provider.GetRequiredService<ContentLoader>();
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            this.Bounds.Height = 55;

            _label = this.children.Create<TextElement>(l =>
            {
                l.Inline = false;
                l.Font = _content.TryGet<SpriteFont>("font:ui:label");
                l.Color = Color.White;
            });

            _input = this.children.Create<TextInput>(i =>
            {
                i.Font = _content.TryGet<SpriteFont>("font:ui:input");
                i.Bounds.Set(0, 30, 1f, 30);

                i.BorderColor = new Color(0, 143, 241);
                i.BorderSize = 1;
                //i.BackgroundColor = new Color(255, 255, 255, 100);
                i.Color = Color.White;

                i.OnActiveChanged += (s, a) =>
                {
                    i.BorderColor = a ? new Color(0, 143, 241) : new Color(0, 143, 241);
                };
            });
        }
        #endregion
    }
}
