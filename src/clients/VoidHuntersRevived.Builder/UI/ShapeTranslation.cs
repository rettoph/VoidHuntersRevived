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
    public class ShapeTranslation : SecretContainer<FormComponent, StackContainer<FormComponent>>
    {
        #region Public Properties
        public FormComponent X { get; private set; }
        public FormComponent Y { get; private set; }
        public FormComponent R { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.inner.Alignment = StackAlignment.Horizontal;

            this.X = this.inner.Children.Create<FormComponent>((angle, p, c) =>
            {
                angle.Label.Value = "X";
                angle.Input.Value = "0";
                angle.Bounds.Width = 1f / 3f;
                angle.Input.Filter = new Regex("^[0-9\\.-]{0,25}$");
            });

            this.Y = this.inner.Children.Create<FormComponent>((angle, p, c) =>
            {
                angle.Label.Value = "Y";
                angle.Input.Value = "0";
                angle.Bounds.Width = 1f / 3f;
                angle.Input.Filter = new Regex("^[0-9\\.-]{0,25}$");
            });

            this.R = this.inner.Children.Create<FormComponent>((angle, p, c) =>
            {
                angle.Label.Value = "R";
                angle.Input.Value = "0";
                angle.Bounds.Width = 1f / 3f;
                angle.Input.Filter = new Regex("^[0-9\\.-]{0,25}$");
            });
        }
        #endregion
    }
}
