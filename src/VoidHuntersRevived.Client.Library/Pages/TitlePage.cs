using Guppy.DependencyInjection;
using Guppy.UI.Components;
using Guppy.UI.Enums;
using Guppy.UI.Utilities.Units;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Pages
{
    public class TitlePage : ProtectedContainer
    {
        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            var titleContainer = this.children.Create<StackContainer>();
            titleContainer.Direction = Direction.Horizontal;
            titleContainer.Bounds.X = new CustomUnit(p => (p - titleContainer.Bounds.Pixel.Width) / 2);
            titleContainer.Bounds.Y = 50;
            titleContainer.Bounds.Height = 100;

            var title1 = titleContainer.Children.Create<Label>("ui:header:1");
            title1.Text = "Void Hunters ";

            var title2 = titleContainer.Children.Create<Label>("ui:header:1");
            title2.Text = "Revived";
        }
        #endregion
    }
}
