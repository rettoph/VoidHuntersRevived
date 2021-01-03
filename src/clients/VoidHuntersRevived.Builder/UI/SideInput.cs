using Guppy.DependencyInjection;
using Guppy.UI.Elements;
using Guppy.UI.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using VoidHuntersRevived.Client.Library.UI;
using Guppy.Extensions.DependencyInjection;
using tainicom.Aether.Physics2D;

namespace VoidHuntersRevived.Builder.UI
{
    public class SideInput : SecretContainer<FormComponent, StackContainer<FormComponent>>
    {
        #region Public Properties
        public FormComponent Angle { get; private set; }
        public FormComponent Length { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.inner.Alignment = StackAlignment.Horizontal;

            this.Angle = this.inner.Children.Create<FormComponent>((angle, p, c) =>
            {
                angle.Label.Value = "Angle";
                angle.Input.Value = (180 - (360 / Settings.MaxPolygonVertices - 1)).ToString();
                angle.Bounds.Width = 0.5f;
                angle.Input.Filter = new Regex("^[0-9\\.]{0,25}$");
            });

            this.Length = this.inner.Children.Create<FormComponent>((length, p, c) =>
            {
                length.Label.Value = "Length";
                length.Input.Value = "1";
                length.Bounds.Width = 0.5f;
                length.Input.Filter = new Regex("^[0-9\\.]{0,25}$");
            });
        }
        #endregion
    }
}
