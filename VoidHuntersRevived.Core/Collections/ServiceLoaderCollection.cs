using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Collections
{
    class ServiceLoaderCollection : InitializableGameCollection<IServiceLoader>, IServiceLoader
    {
        private ILogger _logger;
        private Boolean _configuredServices;
        public IGame Game { get; private set; }
        public event EventHandler<IServiceLoader> OnConfigureServices;

        public ServiceLoaderCollection(ILogger logger) : base(logger)
        {
            _logger = logger;
        }

        

        #region Initializable methods
        protected override void Boot()
        {
            _logger.LogDebug("Booting all ServiceLoaders");

            foreach (IServiceLoader loader in this)
                loader.TryBoot();
        }

        protected override void PreInitialize()
        {
            _logger.LogDebug("PreInitialzing all ServiceLoaders");

            foreach (IServiceLoader loader in this)
                loader.TryPreInitialize();
        }

        protected override void Initialize()
        {
            _logger.LogDebug("Initialzing all ServiceLoaders");

            foreach (IServiceLoader loader in this)
                loader.TryInitialize();
        }

        protected override void PostInitialize()
        {
            _logger.LogDebug("PostInitialzing all ServiceLoaders");

            foreach (IServiceLoader loader in this)
                loader.TryPostInitialize();
        }
        #endregion

        #region IServiceLoader methods
        public void TryConfigureServices(IServiceCollection services)
        {
            if (_configuredServices == true)
                _logger.LogDebug("Unable to call ServiceLoaderCollection.ConfigureServices() more than once.");
            else
            {
                _configuredServices = true;
                _logger.LogDebug("Configuring services in all ServiceLoaders");

                // Configure services on all internal ServiceLoaders
                foreach (IServiceLoader loader in this)
                    loader.TryConfigureServices(services);

                this.OnConfigureServices?.Invoke(this, this);
            }
        }
        #endregion
    }
}