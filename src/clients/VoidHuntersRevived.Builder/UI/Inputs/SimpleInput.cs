using Guppy.DependencyInjection;
using Guppy.UI.Elements;
using Guppy.UI.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Builder.UI.Inputs
{
    public abstract class SimpleInput<T> : BaseLabeledInput<T>
    {
        #region Protexted Properties
        protected TextInput input { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.input = this.inner.Children.Create<TextInput>((input, p, c) =>
            {
                input.Color[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderColor[ElementState.Default] = p.GetColor("ui:color:1");
                input.BorderWidth[ElementState.Default] = 1;
                input.Inline = InlineType.None;
            
                input.Bounds.Width = 1f;
                input.Bounds.Height = 35;
                input.Bounds.Y = 25;
                input.Padding.Left = 7;
                input.Padding.Right = 7;
            });
        }
        #endregion
    }
}
