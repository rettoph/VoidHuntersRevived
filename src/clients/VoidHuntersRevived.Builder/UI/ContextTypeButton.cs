using Guppy.DependencyInjection;
using Guppy.UI.Elements;
using Guppy.UI.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Extensions.DependencyInjection;
using Guppy.UI.Interfaces;

namespace VoidHuntersRevived.Builder.UI
{
    public class ContextTypeButton : SecretContainer<IElement>
    {
        #region Private Fields 
        private TextElement _button;
        #endregion

        #region Public Properties
        public Type ContextType { get; set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);


        }
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            this.Padding.Bottom = 15;
            this.inline = InlineType.Both;

            _button = this.inner.Children.Create<TextElement>("ui:button:0");
            _button.Value = this.ContextType.Name;
            _button.Bounds.Width = 350;
        }
        #endregion
    }
}
