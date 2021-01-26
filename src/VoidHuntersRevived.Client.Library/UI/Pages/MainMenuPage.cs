using Guppy.DependencyInjection;
using Guppy.UI.Elements;
using Guppy.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.UI.Enums;
using System.Text.RegularExpressions;
using Guppy.UI.Utilities.Units;
using Guppy.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Client.Library.UI.Pages
{
    public class MainMenuPage : SecretContainer<IElement>, IPage
    {
        #region Private Fields
        private FormComponent _username;
        private FormComponent _host;
        private FormComponent _port;
        #endregion

        #region Public Properties
        public String Username => _username.Input.Value;
        public String Host => _host.Input.Value;
        public Int32 Port => Int32.Parse(_port.Input.Value);
        public TextElement ConnectButton { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.inner.Children.Create<StackContainer>((container, p, c) =>
            {
                container.Bounds.X = 0;
                container.Bounds.Y = 0.05f;
                container.Alignment = StackAlignment.Vertical;
                container.Inline = InlineType.Vertical;

                container.Children.Create<HeaderComponent>();
                _username = container.Children.Create<FormComponent>((username, p, c) =>
                {
                    username.Label.Value = "Username";
                    username.Input.Value = "Rettoph";
                    username.Bounds.Width = 700;
                    username.Input.Filter = new Regex("^[a-zA-Z0-9]{0,25}$");
                });
                container.Children.Create<StackContainer>((container2, p, c) =>
                {
                    container2.Bounds.X = new CustomUnit(c => (c - container2.Bounds.Width.ToPixel(c)) / 2);
                    container2.Alignment = StackAlignment.Horizontal;
                    _host = container2.Children.Create<FormComponent>((host, p, c) =>
                    {
                        host.Label.Value = "Host";
                        host.Input.Value = "localhost";
                        host.Bounds.Width = 550;
                        host.Input.Filter = new Regex("^[a-zA-Z0-9\\.]{0,100}$");
                    });
                    _port = container2.Children.Create<FormComponent>((port, p, c) =>
                    {
                        port.Label.Value = "Port";
                        port.Input.Value = "1337";
                        port.Bounds.Width = 150;
                        port.Input.Filter = new Regex("^[0-9]{0,5}$");
                    });
                });

                container.Children.Create<Container>((container3, p, c) =>
                {
                    container3.Padding.Top = 25;
                    container3.Inline = InlineType.Vertical;

                    this.ConnectButton = container3.Children.Create<TextElement>((connect, p, c) =>
                    {
                        connect.Color[ElementState.Default] = Color.White;
                        connect.BackgroundColor[ElementState.Default] = p.GetColor("ui:input:color:2");
                        connect.BackgroundColor[ElementState.Hovered] = Color.Lerp(p.GetColor("ui:input:color:2"), Color.Black, 0.25f);
                        connect.BackgroundColor[ElementState.Pressed] = Color.Lerp(p.GetColor("ui:input:color:2"), Color.Black, 0.5f);
                        connect.Inline = InlineType.None;
                        connect.Bounds.Width = 680;
                        connect.Bounds.Height = 45;
                        connect.Bounds.X = new CustomUnit(c => (c - connect.Bounds.Width.ToPixel(c)) / 2);
                        connect.Alignment = Alignment.CenterCenter;
                        connect.Font = p.GetContent<SpriteFont>("font:ui:normal");
                        connect.Value = "Connect";
                    });
                });
            });
        }

        protected override void PostRelease()
        {
            base.PostRelease();

            _username = null;
            _host = null;
            _port = null;
            this.ConnectButton = null;
        }
        #endregion
    }
}
