using Guppy;
using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Builder.UI.Pages;
using VoidHuntersRevived.Library.Contexts;

namespace VoidHuntersRevived.Builder.Services
{
    /// <summary>
    /// The base class representing a service capable of designing a
    /// ShipPartContext instance. Extend this type and add the
    /// ShipPartContextBuilderService Attribute to automatically load
    /// the service when creating a new context instance.
    /// </summary>
    public abstract class ShipPartContextBuilderService : Frameable
    {
        #region Private Fields
        private ShipPartContextBuilderPage _contextBuilderPage;
        #endregion

        #region Protected Properties
        protected PageContainer pages => _contextBuilderPage.Pages;

        protected virtual ShipPartContext context { get; set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _contextBuilderPage);
        }

        protected override void Release()
        {
            base.Release();

            _contextBuilderPage = null;

            this.Close();
        }
        #endregion

        #region Methods
        protected internal virtual void Open(ShipPartContext context)
        {
            this.context = context;
        }

        protected internal virtual void Close()
        {

        }
        #endregion
    }
}
