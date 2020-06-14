using Guppy.DependencyInjection;
using Guppy.UI.Components;
using Guppy.UI.Enums;
using Guppy.UI.Interfaces;
using Guppy.UI.Utilities.Units;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Pages
{
    public class TitlePage : Page
    {
        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            var headerContainer = this.children.Create<StackContainer<IComponent>>();
            headerContainer.Direction = Direction.Horizontal;
            headerContainer.Bounds.X = new CustomUnit(p => (p - headerContainer.Bounds.Pixel.Width) / 2);
            headerContainer.Bounds.Y = 50;
            headerContainer.Bounds.Height = 75;

            var logo = headerContainer.Children.Create<Component>("ui:logo");

            var textContainer = headerContainer.Children.Create<StackContainer<IComponent>>();
            textContainer.Direction = Direction.Vertical;
            textContainer.Inline = true;

            var titleContainer = textContainer.Children.Create<StackContainer<IComponent>>();
            titleContainer.Direction = Direction.Horizontal;
            titleContainer.Bounds.Height = 50;

            var title1 = titleContainer.Children.Create<Label>("ui:header:1");
            title1.Text = "Void Hunters ";

            var title2 = titleContainer.Children.Create<Label>("ui:header:1");
            title2.Text = "Revived";

            var subTitle1 = textContainer.Children.Create<Label>("ui:header:2");
            subTitle1.Text = "Version 0.0.2";
            subTitle1.Bounds.Height = 25;
        }
        #endregion
    }
}
