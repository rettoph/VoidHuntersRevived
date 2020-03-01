using Guppy.Loaders;
using Guppy.UI.Components;
using Guppy.UI.Enums;
using Guppy.UI.Utilities.Units;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Entities.UI
{
    public class Header : ProtectedContainer<Element>
    {
        private ContentLoader _content;

        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _content = provider.GetRequiredService<ContentLoader>();
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            this.children.Create<StackContainer>(c1 =>
            {
                c1.Method = StackMethod.Horizontal;
                c1.Alignment = Alignment.Center;

                // Add the logo...
                c1.Children.Create<FancyElement>(l =>
                {
                    l.Bounds.Set(0, 0, 81, 81);
                    l.BackgroundImage = _content.TryGet<Texture2D>("sprite:logo");
                    l.BackgroundStyle = BackgroundStyle.Fill;
                });

                c1.Children.Create<StackContainer>(c2 =>
                {
                    c2.Alignment = Alignment.CenterRight;

                    c2.Children.Create<StackContainer>(c3 =>
                    {
                        c3.Method = StackMethod.Horizontal;

                        c3.Children.Create<TextElement>(t1 =>
                        {
                            t1.Text = "Void Hunters";
                            t1.Font = _content.TryGet<SpriteFont>("font:ui:title");
                        });

                        c3.Children.Create<TextElement>(t2 =>
                        {
                            t2.Text = " Revived";
                            t2.Font = _content.TryGet<SpriteFont>("font:ui:title-light");
                            t2.Color = new Color(0, 143, 241);
                        });
                    });

                    c2.Children.Create<TextElement>(t3 =>
                    {
                        t3.Text = "Alpha 0.0.1";
                        t3.Font = _content.TryGet<SpriteFont>("font:ui:input");
                        t3.Color = Color.Gray;
                    });
                });
                
            });
        }
    }
}
