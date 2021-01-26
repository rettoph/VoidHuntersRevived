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
using Microsoft.Win32;
using Microsoft.Extensions.DependencyInjection;
using ServiceProvider = Guppy.DependencyInjection.ServiceProvider;
using VoidHuntersRevived.Library.Services;
using System.IO;

namespace VoidHuntersRevived.Builder.UI.Pages
{
    public class ShipPartContextSelectorPage : SecretContainer<IElement>, IPage
    {
        #region Private Fields
        private StackContainer _stack;
        private List<ContextTypeButton> _buttons;
        private TextElement _import;
        private ServiceProvider _provider;
        private ShipPartService _shipParts;
        #endregion

        #region Events
        public event OnEventDelegate<ShipPartContextSelectorPage, ShipPartContext> OnContextSelected;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            _provider = provider;
            provider.Service(out _shipParts);

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

            _import = _stack.Children.Create<TextElement>("ui:button:0", (b, p, c) =>
            {
                b.Value = "Import File";
                b.Bounds.Width = 350;
                b.OnClicked += this.HandleImportButtonClicked;
            });
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
            => this.OnContextSelected?.Invoke(
                sender: this, 
                args: ActivatorUtilities.CreateInstance(
                    provider: _provider, 
                    instanceType: (sender as ContextTypeButton).ContextType, 
                    ""
                ) as ShipPartContext);

        private void HandleImportButtonClicked(Element sender)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "ShipPart files|*.vhsp";
            dialog.InitialDirectory = $"{Environment.CurrentDirectory}\\Resources\\ShipParts";
            // dialog.InitialDirectory = ".";

            if (dialog.ShowDialog() ?? false)
            {
                using (Stream contextStream = dialog.OpenFile())
                    this.OnContextSelected?.Invoke(
                        sender: this,
                        args: _shipParts.TryRegister(contextStream));
            }
        }
        #endregion
    }
}
