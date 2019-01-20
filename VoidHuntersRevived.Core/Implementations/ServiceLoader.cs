using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VoidHuntersRevived.Core.Interfaces;

namespace VoidHuntersRevived.Core.Implementations
{
    public abstract class ServiceLoader : Initializable, IServiceLoader
    {
        private ILogger _logger;
        private Boolean _configuredServices;
        public IGame Game { get; private set; }

        public event EventHandler<IServiceLoader> OnConfigureServices;

        public ServiceLoader(ILogger logger, IGame game = null)
            : base(logger)
        {
            _configuredServices = false;
            _logger = logger;

            this.Game = game;
        }

        public void TryConfigureServices(IServiceCollection services)
        {
            if (_configuredServices == true)
                _logger.LogDebug("Unable to call ServiceLoader.ConfigureServices() more than once.");
            else
            {
                _configuredServices = true;
                _logger.LogDebug("Calling ServiceLoader.ConfigureServices()");
                this.ConfigureServices(services);
                this.OnConfigureServices?.Invoke(this, this);
            }
        }

        protected abstract void ConfigureServices(IServiceCollection services);
    }
}
