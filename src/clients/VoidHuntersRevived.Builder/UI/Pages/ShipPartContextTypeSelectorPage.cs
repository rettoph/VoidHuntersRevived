using Guppy.DependencyInjection;
using Guppy.UI.Elements;
using Guppy.UI.Interfaces;
using Guppy.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Contexts;
using Guppy.Extensions.System;
using Guppy.Extensions.System.Collections;
using System.Linq;
using Guppy.UI.Enums;
using Guppy.UI.Utilities.Units;
using Guppy.Events.Delegates;

namespace VoidHuntersRevived.Builder.UI.Pages
{
    public class ShipPartContextTypeSelectorPage : SecretContainer<IElement>, IPage
    {
        #region Private Fields
        private StackContainer _stack;
        private List<ContextTypeButton> _buttons;
        #endregion

        #region Events
        public event OnEventDelegate<ShipPartContextTypeSelectorPage, Type> OnContextTypeSelected;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            // Create a placeholder stack container
            _stack = this.inner.Children.Create<StackContainer>((stack, p, c) =>
            {
                stack.Alignment = StackAlignment.Vertical;
                stack.Bounds.Width = 350;
                stack.Bounds.Y = 25;
                stack.Bounds.X = new CustomUnit(c => (c - stack.Bounds.Width.ToPixel(c)) / 2);
            });

            // Create buttons for each type of valid context type.
            _buttons = AssemblyHelper.Types.GetTypesWithAttribute<ShipPartContext, ShipPartContextAttribute>(false)
                .Select(t => _stack.Children.Create<ContextTypeButton>((b, p, c) =>
                {
                    b.ContextType = t;
                    b.OnClicked += this.HandleContextTypeButtonClicked;
                }))
                .ToList();
        }

        protected override void Release()
        {
            base.Release();

            _stack = null;
            _buttons.Clear();
        }
        #endregion

        #region Event Handlers
        private void HandleContextTypeButtonClicked(Element sender)
            => this.OnContextTypeSelected?.Invoke(this, (sender as ContextTypeButton).ContextType);
        #endregion
    }
}
