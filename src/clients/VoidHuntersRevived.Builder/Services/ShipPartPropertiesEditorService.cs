using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Builder.Attributes;
using VoidHuntersRevived.Builder.UI.Pages;
using VoidHuntersRevived.Library.Contexts;
using Guppy.Extensions.System.Reflection;
using VoidHuntersRevived.Library.Attributes;
using System.Reflection;

namespace VoidHuntersRevived.Builder.Services
{
    /// <summary>
    /// Simple service that will utilize 
    /// reflection to define ShipPartContext 
    /// property value.
    /// </summary>
    [ShipPartContextBuilderService("Properties", 1)]
    public class ShipPartPropertiesEditorService : ShipPartContextBuilderService
    {
        #region Private Fields
        private ShipPartPropertiesEditorPage _page;
        private List<PropertyInfo> _properties;
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            // Create a brand new page for the defined main stage...
            _page = this.pages.Children.Create<ShipPartPropertiesEditorPage>();
        }

        protected override void Release()
        {
            base.Release();

            _page.TryRelease();

            _page = null;
        }
        #endregion

        #region Helper Methods 
        protected internal override void Open(ShipPartContext context)
        {
            base.Open(context);

            // Load all editable properties...
            _properties = context.GetType()
                .GetProperties()
                .Where(pi => pi.HasCustomAttribute<ShipPartContextPropertyAttribute>())
                .ToList();


            // Open the default API page...
            _page.LoadProperties(context, _properties);
            this.pages.Open(_page);
        }

        protected internal override void Close()
        {
            _page.UnloadProperties();
            base.Close();
        }
        #endregion
    }
}
